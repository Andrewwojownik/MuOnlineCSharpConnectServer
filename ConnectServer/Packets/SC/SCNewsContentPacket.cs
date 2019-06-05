using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectServer.Packets.SC
{
    class NewsContentPacket : PacketHandler
    {
        public byte[] CreatePacket()
        {
            SCNewsContentPacket packet = new SCNewsContentPacket();
            LongPlainPacketHeader head = new LongPlainPacketHeader();
            head.Type = Type.LONG_PLAIN;
            head.HeadCode = HeadCodeSC.CONNECT_SERVER_CUSTOM_DATA;
            head.HeadSubCode = HeadSubCodeSC.NEWS_CONTENT;
            //head.Size = (byte)System.Runtime.InteropServices.Marshal.SizeOf(typeof(SCNewsTitlePacket));

            packet.Day = 1;
            packet.Month = 1;
            packet.Year = 2019;

            packet.DateColor = 0xFFFFFFFF;
            packet.TitleColor = 0xFFFFFFFF;
            packet.TextColor = 0xFFFFFFFF;

            packet.Title = "Test";

            packet.TextSize = 0;

            head.SetSize((ushort)System.Runtime.InteropServices.Marshal.SizeOf(typeof(SCNewsContentPacket)));
            packet.Head = head;

            return AsynchronousSocketListener.getBytes(packet);
        }
    }
    struct SCNewsContentPacket : Packet
    {
        public LongPlainPacketHeader Head { get; set; }
        public byte Day { get; set; }
        public byte Month { get; set; }
        public ushort Year { get; set; }

        public ulong DateColor { get; set; }
        public ulong TitleColor { get; set; }
        public ulong TextColor { get; set; }
        [MarshalAs(UnmanagedType.LPTStr, SizeConst = 20)]
        public string Title;
        public ushort TextSize { get; set; }
    }
}
