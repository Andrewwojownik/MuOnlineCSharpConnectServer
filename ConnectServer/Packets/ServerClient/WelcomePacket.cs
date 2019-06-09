namespace ConnectServer.Packets.SC
{
    internal class WelcomePacket : ICreatePacketHandler
    {
        public SendPacket CreatePacket()
        {
            ShortPlainPacketHeader head = new ShortPlainPacketHeader
            {
                Type = Type.ShortPlain,
                HeadCode = HeadCodeSc.Welcome,
                Size = 4
            };

            ScWelcomePacket packet = new ScWelcomePacket { Head = head, Result = 1 };

            return new SendPacket { Size = packet.Head.Size, Packet = packet.GetBytes() };
        }
    }
    internal struct ScWelcomePacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public byte Result { set; get; }
    }
}
