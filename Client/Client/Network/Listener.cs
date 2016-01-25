using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using Client.Network.Packets;

namespace Client.Network
{
    class Listener
    {
        private Socket socket;
        IPAddress hostIP;

        private const int listenPort = 11011;
        UdpClient listener;
        IPEndPoint hostEP;

        //List<string> messageLog = new List<string>();

        public Listener(String ip)
        {
            try
            {
                hostIP = IPAddress.Parse(ip);
                listener = new UdpClient(listenPort);
                hostEP = new IPEndPoint(hostIP, listenPort);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void sendPacket(Packet packet)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            byte[] data = Deconstruct(packet);
            IPEndPoint endPoint = new IPEndPoint(hostIP, 11000);
            socket.SendTo(data, endPoint);
        }

        public void listen()
        {
            try
            {
                byte[] data = listener.Receive(ref hostEP);
                Packet incoming = Reconstruct(data);
                if (incoming is Packet000)
                {
                    //This should never happen. NEVER!
                }
                else if (incoming is Packet001)
                {
                    Packet001 msgPacket = (Packet001)incoming;
                    string message = msgPacket.getMessage();
                    Console.ForegroundColor = msgPacket.getForeground();
                    //Console.BackgroundColor = msgPacket.getBackground();
                    Console.WriteLine(message);
                    Console.ResetColor();
                    //messageLog = msgPacket.getLog();

                }
                else if (incoming is Packet002)
                {
                    Packet002 msgPacket = (Packet002)incoming;
                    string message = msgPacket.getMessage();
                    Console.ForegroundColor = msgPacket.getForeground();
                    //Console.BackgroundColor = msgPacket.getBackground();
                    Console.WriteLine(message);
                    Console.ResetColor();
                    //messageLog = msgPacket.getLog();

                }
                else if (incoming is Packet999)
                {
                    //This should never happen. NEVER!
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

        public string getLocalIP()
        {
            string localIP = "";
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public string getGlobalIP()
        {
            string globalIP = "";
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org");
            using (WebResponse response = request.GetResponse())
            using (StreamReader stream = new StreamReader(response.GetResponseStream()))
            {
                globalIP = stream.ReadToEnd();
            }

            int first = globalIP.IndexOf("Address: ") + 9;
            int last = globalIP.LastIndexOf("</body>");
            globalIP = globalIP.Substring(first, last - first);

            return globalIP;
        }
    }
}