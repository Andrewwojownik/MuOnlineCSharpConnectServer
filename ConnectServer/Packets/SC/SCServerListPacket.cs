using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectServer.Packets.SC
{
    class ServerListPacket : PacketHandler
    {
        public byte[] CreatePacket()
        {
            SCServerListPacket packet = new SCServerListPacket();
            LongPlainPacketHeader head = new LongPlainPacketHeader();
            head.Type = Type.LONG_PLAIN;
            head.HeadCode = HeadCodeSC.CONNECT_SERVER_DATA;
            head.HeadSubCode = HeadSubCodeSC.SERVER_LIST;

            ServerListEntryPart serverListEntryPart = new ServerListEntryPart();
            serverListEntryPart.ServerCode = 0;
            serverListEntryPart.Percent = 0;
            serverListEntryPart.PlayType = 0;

            ServerListEntryPart serverListEntryPart2 = new ServerListEntryPart();
            serverListEntryPart2.ServerCode = 1;
            serverListEntryPart2.Percent = 0;
            serverListEntryPart2.PlayType = 0;

            head.SetSize((ushort)
                (
                System.Runtime.InteropServices.Marshal.SizeOf(typeof(SCServerListPacket))
                + System.Runtime.InteropServices.Marshal.SizeOf(typeof(ServerListEntryPart))
                + System.Runtime.InteropServices.Marshal.SizeOf(typeof(ServerListEntryPart))
                )
                );
            packet.Head = head;
            packet.SetCount(2);

            return AsynchronousSocketListener.CombineByteArray(AsynchronousSocketListener.CombineByteArray(AsynchronousSocketListener.getBytes(packet), AsynchronousSocketListener.getBytes(serverListEntryPart)), AsynchronousSocketListener.getBytes(serverListEntryPart2));
        }
    }

    struct SCServerListPacket : Packet
    {
        public LongPlainPacketHeader Head { get; set; }
        public byte CountH { set; get; }
        public byte CountL { set; get; }
        public void SetCount(ushort count)
        {
            this.CountL = ((byte)(count & 0xff));
            this.CountH = ((byte)(count >> 8));
        }
    }

    struct ServerListEntryPart : Packet
    {
        public ushort ServerCode { set; get; }
        public byte Percent { set; get; }
        public byte PlayType { set; get; }
    }
}
