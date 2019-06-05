namespace ConnectServer.Packets.SC
{
    internal class ServerListPacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            LongPlainPacketHeader head = new LongPlainPacketHeader
            {
                Type = Type.LONG_PLAIN,
                HeadCode = HeadCodeSc.CONNECT_SERVER_DATA,
                HeadSubCode = HeadSubCodeSc.SERVER_LIST
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
                (
                System.Runtime.InteropServices.Marshal.SizeOf(typeof(ScServerListPacket))
                + System.Runtime.InteropServices.Marshal.SizeOf(typeof(ServerListEntryPart))
                + System.Runtime.InteropServices.Marshal.SizeOf(typeof(ServerListEntryPart))
                )
                );
            ScServerListPacket packet = new ScServerListPacket { Head = head };
            packet.SetCount(2);

            return AsynchronousSocketListener.CombineByteArray(AsynchronousSocketListener.CombineByteArray(AsynchronousSocketListener.GetBytes(packet), AsynchronousSocketListener.GetBytes(serverListEntryPart)), AsynchronousSocketListener.GetBytes(serverListEntryPart2));
        }
    }

    internal struct ScServerListPacket : IPacket
    {
        public LongPlainPacketHeader Head { get; set; }
        public byte CountH { set; get; }
        public byte CountL { set; get; }
        public void SetCount(ushort count)
        {
            CountL = ((byte)(count & 0xff));
            CountH = ((byte)(count >> 8));
        }
    }

    internal struct ServerListEntryPart : IPacket
    {
        public ushort ServerCode { set; get; }
        public byte Percent { set; get; }
        public byte PlayType { set; get; }
    }
}
