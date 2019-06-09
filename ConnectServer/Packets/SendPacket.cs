using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectServer.Packets
{
    public struct SendPacket
    {
        public int Size { get; set; }
        public byte[] Packet { get; set; }
    }
}
