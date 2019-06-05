using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectServer.Packets.SC
{
    class WelcomePacket : PacketHandler
    {
        public byte[] CreatePacket()
        {
            SCWelcomePacket packet = new SCWelcomePacket();
            ShortPlainPacketHeader head = new ShortPlainPacketHeader();
            head.Type = Type.SHORT_PLAIN;
            head.HeadCode = HeadCodeSC.WELCOME;
            head.Size = 4;
            packet.Head = head;
            packet.Result = 1;

            return AsynchronousSocketListener.getBytes(packet);
        }
    }
    struct SCWelcomePacket : Packet
    {
        public ShortPlainPacketHeader Head { get; set; }
        public byte Result { set; get; }
    }
}
