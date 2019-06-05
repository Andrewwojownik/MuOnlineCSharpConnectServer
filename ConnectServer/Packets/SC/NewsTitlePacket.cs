using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectServer.Packets.SC
{
    class NewsTitlePacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            SCNewsTitlePacket packet = new SCNewsTitlePacket();
            ShortPlainPacketHeader head = new ShortPlainPacketHeader();
            head.Type = Type.SHORT_PLAIN;
            head.HeadCode = HeadCodeSc.CONNECT_SERVER_CUSTOM_DATA;
            packet.SubCode = HeadSubCodeSc.NEWS_TITLE;
            head.Size = (byte)System.Runtime.InteropServices.Marshal.SizeOf(typeof(SCNewsTitlePacket));
            packet.Head = head;
            string ServerName = "KamikazeMU";
            var ServerNameBytes = new byte[12];
            Array.Clear(ServerNameBytes, 0, ServerNameBytes.Length);
            Array.Copy(Encoding.ASCII.GetBytes(ServerName), 0, ServerNameBytes, 0, ServerName.Length);
            packet.ServerName = ServerNameBytes;

            return AsynchronousSocketListener.GetBytes(packet);
        }
    }
    struct SCNewsTitlePacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public HeadSubCodeSc SubCode { get; set; }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] ServerName;
    }
}
