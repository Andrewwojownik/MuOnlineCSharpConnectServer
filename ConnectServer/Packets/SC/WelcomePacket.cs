namespace ConnectServer.Packets.SC
{
    internal class WelcomePacket : IPacketHandler
    {
        public byte[] CreatePacket()
        {
            ShortPlainPacketHeader head = new ShortPlainPacketHeader
            {
                Type = Type.ShortPlain,
                HeadCode = HeadCodeSc.Welcome,
                Size = 4
            };

            ScWelcomePacket packet = new ScWelcomePacket { Head = head, Result = 1 };

            return AsynchronousSocketListener.GetBytes(packet);
        }
    }
    internal struct ScWelcomePacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public byte Result { set; get; }
    }
}
