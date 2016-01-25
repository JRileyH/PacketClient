using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Client;
using Client.Network;
using Client.Network.Packets;

namespace Server
{
    class Server
    {
        private const int listenPort = 11000;
        private UdpClient listener;
        private IPEndPoint groupEP;
        private Socket socket;
        private List<IPAddress> broadcastTo = new List<IPAddress>();
        private List<String> messageLog = new List<String>();
        private String lastMessage = "waiting";

        public Server()
        {
            listener = new UdpClient(listenPort);
            groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            Console.Write("Server Running\n");
        }

        private void listen()
        {
            try
            {
                byte[] data = listener.Receive(ref groupEP);
                Packet incoming = Reconstruct(data);
                if(incoming is Packet000)
                {
                    Console.Write("Connection: ");
                    Packet000 connectPacket = (Packet000)incoming;
                    IPAddress connectingIP = connectPacket.getSenderIP();
                    Console.WriteLine(connectPacket.getName());
                    Console.WriteLine(connectingIP.ToString() + ": Player has connected.");
                    broadcastTo.Add(connectingIP);
                }
                else if (incoming is Packet001)
                {
                    Console.Write("Message: ");
                    Packet001 msgPacket = (Packet001)incoming;
                    string message = msgPacket.getMessage();
                    IPAddress sender = msgPacket.getSenderIP();
                    ConsoleColor color = msgPacket.getForeground();
                    Console.WriteLine(msgPacket.getName());
                    Console.WriteLine(message);
                    messageLog.Insert(0, lastMessage);
                    lastMessage = message;
                    broadcastExclusively(new Packet001(sender.ToString()+"> " + lastMessage, color), sender);
                }
                else if (incoming is Packet002)
                {
                    Console.Write("Message: ");
                    Packet002 msgPacket = (Packet002)incoming;
                    string message = msgPacket.getMessage();
                    IPAddress sender = msgPacket.getSenderIP();
                    //Console.WriteLine(msgPacket.getRecipient().ToString());
                    IPAddress recipient = msgPacket.getRecipient();
                    if(broadcastTo.Any(e => e.ToString() == recipient.ToString()))
                    {
                        Console.WriteLine(msgPacket.getName());
                        Console.WriteLine(message);
                        messageLog.Insert(0, lastMessage);
                        lastMessage = message;
                        broadcastPrivately(new Packet002("[" + sender.ToString() + "]> " + lastMessage, "null:servermessage", ConsoleColor.Magenta), recipient);
                    }
                    else
                    {
                        Console.WriteLine(msgPacket.getName());
                        Console.WriteLine(message);
                        messageLog.Insert(0, lastMessage);
                        lastMessage = message;
                        broadcastPrivately(new Packet002("[" + recipient.ToString() + " doesn't exist]", "null:servermessage", ConsoleColor.Red), sender);
                    }
                    
                }
                else if (incoming is Packet999)
                {
                    Console.Write("Disconnection: ");
                    Packet999 disconnectPacket = (Packet999)incoming;
                    IPAddress disconnectingIP = disconnectPacket.getSenderIP();
                    Console.WriteLine(disconnectPacket.getName());
                    Console.WriteLine(disconnectingIP.ToString() + ": Player has disconnected.");
                    broadcastTo.Remove(disconnectingIP);
                }
                else
                {
                    Console.WriteLine("Packet Confusion");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void broadcast(Packet packet)
        {
            foreach(IPAddress clientIP in broadcastTo)
            {
                Console.WriteLine("Message Sent to: " + clientIP.ToString());
                sendPacket(packet, clientIP);
            }
        }

        private void broadcastExclusively(Packet packet, IPAddress sender)
        {
            foreach (IPAddress clientIP in broadcastTo)
            {
                if (clientIP.ToString() != sender.ToString())
                {
                    Console.WriteLine("Message Sent to: " + clientIP.ToString());
                    sendPacket(packet, clientIP);
                }
            }
        }

        private void broadcastPrivately(Packet packet, IPAddress recipient)
        {
            sendPacket(packet, recipient);
        }

        private void sendPacket(Packet packet, IPAddress ip)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] data = Deconstruct(packet);
            IPEndPoint endPoint = new IPEndPoint(ip, 11011);
            socket.SendTo(data, endPoint);
        }

        private byte[] Deconstruct(Packet packet)
        {
            if (packet == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, packet);
            return ms.ToArray();
        }

        private Packet Reconstruct(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            ms.Write(data, 0, data.Length);
            ms.Seek(0, SeekOrigin.Begin);
            Packet packet = (Packet)bf.Deserialize(ms);
            return packet;
        }

        static void Main(string[] args)
        {
            Server server = new Server();
            while(true)
            {
                server.listen();
            }
        }
    }
}
