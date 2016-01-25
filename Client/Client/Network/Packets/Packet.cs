using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Client.Network.Packets
{
    [Serializable]
    public class Packet
    {
        protected String packetName;
        protected IPAddress senderIP;
        protected ConsoleColor background = ConsoleColor.Black;
        protected ConsoleColor foreground = ConsoleColor.White;

        public Packet()
        {
            packetName = "ParentPacket";
            senderIP = Program.myIP;
        }

        public String getName()
        {
            return packetName;
        }

        public IPAddress getSenderIP()
        {
            return senderIP;
        }

        public ConsoleColor getForeground()
        {
            return foreground;
        }

        public ConsoleColor getBackground()
        {
            return background;
        }
    }
}