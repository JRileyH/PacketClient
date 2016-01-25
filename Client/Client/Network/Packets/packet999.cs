using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Client.Network.Packets
{
    [Serializable]
    public class Packet999 : Packet
    {
        public Packet999()
        {
            packetName = "DisconnectPacket";
        }
    }
}
