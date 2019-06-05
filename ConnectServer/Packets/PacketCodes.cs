﻿namespace ConnectServer.Packets
{
    public enum Type : byte
    {
        SHORT_PLAIN = 0xC1,
        LONG_PLAIN = 0xC2,
        SHORT_ENCODED = 0xC3,
        LONG_ENCODED = 0xC4,
    }
    public enum HeadCodeCs : byte
    {
        SERVER_SELECY = 3,
        AUTOUPDATE_DATA = 5,
        CLIENT_CONNECT = 6,
    }

    public enum HeadCodeSc : byte
    {
        WELCOME = 0x00,
        CONNECT_SERVER_DATA = 0xF4,
        CONNECT_SERVER_CUSTOM_DATA = 0xFA,
    }

    public enum HeadSubCodeSc : byte
    {
        NEWS_TITLE = 0x00,
        NEWS_CONTENT = 0x01,
        SERVER_DATA = 0x03,
        SERVER_LIST = 0x06,
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
