namespace ConnectServer.Packets
{
    public enum Type : byte
    {
        ShortPlain = 0xC1,
        LongPlain = 0xC2,
        ShortEncoded = 0xC3,
        LongEncoded = 0xC4,
    }
    public enum HeadCodeCs : byte
    {
        ServerSelect = 3,
        AutoupdateData = 5,
        ClientConnect = 6,
    }

    public enum HeadCodeSc : byte
    {
        Welcome = 0x00,
        ConnectServerData = 0xF4,
        ConnectServerCustomData = 0xFA,
    }

    public enum HeadSubCodeSc : byte
    {
        NewsTitle = 0x00,
        NewsContent = 0x01,
        ServerData = 0x03,
        ServerList = 0x06,
    }

    public struct ShortPlainPacketHeader
    {
        public Type Type { get; set; }
        public byte Size { get; set; }
        public HeadCodeSc HeadCode { get; set; }
    }

    public struct LongPlainPacketHeader
    {
        public Type Type { get; set; }
        public byte SizeH { get; set; }
        public byte SizeL { get; set; }
        public HeadCodeSc HeadCode { get; set; }
        public HeadSubCodeSc HeadSubCode { get; set; }
        public void SetSize(ushort size)
        {
            SizeL = (byte)(size & 0xff);
            SizeH = (byte)(size >> 8);
        }
    }

    public interface IPacketHandler
    {
        byte[] CreatePacket();
    }

    public interface IPacket
    {
    }

}
