using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectServer.Packets.SC
{
    internal class NewsTitlePacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            ScNewsTitlePacket packet = new ScNewsTitlePacket
            {
                SubCode = HeadSubCodeSc.NewsTitle,
                Head = new ShortPlainPacketHeader
                {
                    Type = Type.ShortPlain,
                    HeadCode = HeadCodeSc.ConnectServerCustomData,
                    Size = (byte) Marshal.SizeOf(typeof(ScNewsTitlePacket))
                }
            };
            const string serverName = "KamikazeMU";
            packet.ServerName = Program.ConvertStringToBytes(serverName, 12);

            return packet.GetBytes();
        }
    }

    internal struct ScNewsTitlePacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public HeadSubCodeSc SubCode { get; set; }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] ServerName;
    }
}
