using System.Runtime.InteropServices;

namespace ConnectServer.Packets.SC
{
    internal class NewsContentPacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            LongPlainPacketHeader head = new LongPlainPacketHeader
            {
                Type = Type.LongPlain,
                HeadCode = HeadCodeSc.ConnectServerCustomData,
                HeadSubCode = HeadSubCodeSc.NewsContent
            };

            head.SetSize((ushort)Marshal.SizeOf(typeof(ScNewsContentPacket)));

            ScNewsContentPacket packet = new ScNewsContentPacket
            {
                Day = 1,
                Month = 1,
                Year = 2019,
                DateColor = 0xFFFFFFFF,
                TitleColor = 0xFFFFFFFF,
                TextColor = 0xFFFFFFFF,
                Title = "Test",
                Head = head,
                TextSize = 0
            };

            return packet.GetBytes();
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
