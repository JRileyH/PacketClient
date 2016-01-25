using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Client.Network.Packets
{
    [Serializable]
    public class Packet000 : Packet
    {
        public Packet000()
        {
            packetName = "ConnectionPacket";
        }
    }
}
