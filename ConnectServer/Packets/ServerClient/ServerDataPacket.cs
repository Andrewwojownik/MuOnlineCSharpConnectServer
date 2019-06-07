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
                Type = Type.LongPlain,
                HeadCode = HeadCodeSc.ConnectServerData,
            };

            ServerObject chosenServer;

            foreach (ServerObject server in server.Servers)
            {
                if(server.ServerCode == this.packet.ServerCode)
                {
                    chosenServer = server;
                }
            }

            ScServerDataPacket packet = new ScServerDataPacket()
            {
                Head = head,
                SubCode = HeadSubCodeSc.ServerData,
                //Port = chosenServer.Port,
            };

            const string IP = "KamikazeMU";
            packet.IP = Program.ConvertStringToBytes(IP, 16);

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
