using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
namespace Client.Network.Packets
{
    [Serializable]
    public class Packet002 : Packet
    {
        private string message;
        private IPAddress recipient;

        public Packet002(string msg, string rec)
        {
            packetName = "PrivateMessagePacket";
            message = msg;
            if(rec != "null:servermessage")
            {
                recipient = IPAddress.Parse(rec);
            }
        }

        public Packet002(string msg, string rec, ConsoleColor color)
        {
            foreground = color;
            packetName = "PrivateMessagePacket";
            message = msg;
            if (rec != "null:servermessage")
            {
                recipient = IPAddress.Parse(rec);
            }
        }

        public string getMessage()
        {
            return message;
        }

        public IPAddress getRecipient()
        {
            return recipient;
        }
    }
}
