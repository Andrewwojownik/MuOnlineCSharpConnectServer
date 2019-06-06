using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectServer.Packets.SC
{
    internal class NewsContentPacket : ICreatePacketHandler
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
                Head = head,
                TextSize = 0
            };

            const string newsTitle = "Example news";
            packet.Title = Program.ConvertStringToBytes(newsTitle, 20);

            const string newsContent = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Proin eu luctus massa, vitae bibendum arcu. Etiam a enim eget quam vulputate luctus. Nullam sed nisi posuere, aliquet lacus mattis, condimentum metus. Pellentesque tincidunt ut ante vel venenatis. Nulla elementum placerat mi ac venenatis. Ut eleifend tellus et tellus euismod auctor. Ut dignissim, arcu id pulvinar auctor, odio est tincidunt justo, sed sagittis ligula nisl sit amet nulla. Integer sed tempus arcu, vel cursus velit. Vestibulum in commodo dolor. Morbi eu mi orci. ";
            packet.Content = Program.ConvertStringToBytes(newsContent, 2048);

            packet.TextSize = 538;

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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] Title;
        public ushort TextSize { get; set; }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
        public byte[] Content;
    }
}
