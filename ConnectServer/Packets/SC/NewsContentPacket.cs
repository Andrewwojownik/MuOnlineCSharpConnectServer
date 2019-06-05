using System.Runtime.InteropServices;

namespace ConnectServer.Packets.SC
{
    internal class NewsContentPacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            ScNewsContentPacket packet = new ScNewsContentPacket();
            LongPlainPacketHeader head = new LongPlainPacketHeader
            {
                Type = Type.LONG_PLAIN,
                HeadCode = HeadCodeSc.CONNECT_SERVER_CUSTOM_DATA,
                HeadSubCode = HeadSubCodeSc.NEWS_CONTENT
            };
            //head.Size = (byte)System.Runtime.InteropServices.Marshal.SizeOf(typeof(SCNewsTitlePacket));

            packet.Day = 1;
            packet.Month = 1;
            packet.Year = 2019;

            packet.DateColor = 0xFFFFFFFF;
            packet.TitleColor = 0xFFFFFFFF;
            packet.TextColor = 0xFFFFFFFF;

            packet.Title = "Test";

            packet.TextSize = 0;

            head.SetSize((ushort)Marshal.SizeOf(typeof(ScNewsContentPacket)));
            packet.Head = head;

            return AsynchronousSocketListener.GetBytes(packet);
        }
    }

    internal struct ScNewsContentPacket : IPacket
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
