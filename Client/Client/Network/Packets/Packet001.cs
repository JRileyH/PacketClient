using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.Network.Packets
{
    [Serializable]
    public class Packet001 : Packet
    {
        private string message;
        private List<string> messageLog;

        public Packet001(string msg)
        {
            packetName = "MessagePacket";
            message = msg;
        }
        public Packet001(string msg, ConsoleColor color)
        {
            foreground = color;
            packetName = "MessagePacket";
            message = msg;
        }
        public Packet001(string msg, List<string> log)
        {
            packetName = "MessagePacket";
            message = msg;
            messageLog = log;
        }

        public string getMessage()
        {
            return message;
        }

        public List<string> getLog()
        {
            return messageLog;
        }

        public string getLogMessage(int index)
        {
            return messageLog.ElementAt(index);
        }
    }
}