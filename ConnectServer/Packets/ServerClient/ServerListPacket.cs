using System.Runtime.InteropServices;

namespace ConnectServer.Packets.SC
{
    internal class ServerListPacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            LongPlainPacketHeader head = new LongPlainPacketHeader
            {
                Type = Type.LongPlain,
                HeadCode = HeadCodeSc.ConnectServerData,
                HeadSubCode = HeadSubCodeSc.ServerList
            };

            ServerListEntryPart serverListEntryPart = new ServerListEntryPart
            {
                ServerCode = 0,
                Percent = 0,
                PlayType = 0
            };

            ServerListEntryPart serverListEntryPart2 = new ServerListEntryPart
            {
                ServerCode = 1,
                Percent = 0,
                PlayType = 0
            };

            head.SetSize((ushort)
                (Marshal.SizeOf(typeof(ScServerListPacket))
                + Marshal.SizeOf(typeof(ServerListEntryPart))
                + Marshal.SizeOf(typeof(ServerListEntryPart))));

            ScServerListPacket packet = new ScServerListPacket { Head = head };
            packet.SetCount(2);

            return packet.GetBytes()
                .Combine(serverListEntryPart.GetBytes())
                .Combine(serverListEntryPart2.GetBytes());
        }
    }

    internal struct ScServerListPacket : IPacket
    {
        public LongPlainPacketHeader Head { get; set; }
        public byte CountH { set; get; }
        public byte CountL { set; get; }
        public void SetCount(ushort count)
        {
            CountL = (byte)(count & 0xff);
            CountH = (byte)(count >> 8);
        }
    }

    internal struct ServerListEntryPart : IPacket
    {
        public ushort ServerCode { set; get; }
        public byte Percent { set; get; }
        public byte PlayType { set; get; }
    }
}
