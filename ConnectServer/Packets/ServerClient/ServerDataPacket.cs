using ConnectServer.ServerConnector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ConnectServer.Packets.ServerClient
{
    internal class ServerDataPacket : ICreatePacketHandler
    {
        private ServerConnector.Server server;
        private CsServerDataPacket packet;
        public ServerDataPacket(Server server, byte[] rawPacket)
        {
            this.server = server;
            this.packet = Program.ByteArrayToStructure<CsServerDataPacket>(rawPacket);
        }
        public byte[] CreatePacket()
        {
            ShortPlainPacketHeader head = new ShortPlainPacketHeader
            {
                Type = Type.ShortPlain,
                HeadCode = HeadCodeSc.ConnectServerData,
                Size = (byte)Marshal.SizeOf(typeof(ScServerDataPacket))
            };

            ServerObject chosenServer = null;

            foreach (ServerObject server in server.Servers)
            {
                if(server.ServerCode == this.packet.ServerCode)
                {
                    chosenServer = server;
                }
            }

            if(chosenServer == null)
            {
                throw new Exception("Ask for non exist server!");
            }

            ScServerDataPacket packet = new ScServerDataPacket()
            {
                Head = head,
                SubCode = HeadSubCodeSc.ServerData,
                Port = (ushort)chosenServer.Port,
            };

            packet.IP = Program.ConvertStringToBytes(chosenServer.IP, 16);

            return packet.GetBytes();
        }
    }

    internal struct ScServerDataPacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public HeadSubCodeSc SubCode { set; get; }
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] IP;
        public ushort Port { set; get; }
    }

    internal struct CsServerDataPacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public HeadSubCodeSc SubCode { set; get; }
        public ushort ServerCode { get; set; }
    }
}
