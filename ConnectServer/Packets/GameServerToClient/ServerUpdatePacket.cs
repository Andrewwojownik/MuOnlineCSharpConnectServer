using ConnectServer.ServerConnector;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConnectServer.Packets.GameServerToClient
{
    class ServerUpdatePacket : IIncomingPacketHandler
    {
        private Server server;

        public ServerUpdatePacket(Server server)
        {
            this.server = server;
        }

        public void Handle(byte[] rawPacket)
        {
            ServerDataUpdatePacket packet = Program.ByteArrayToStructure<ServerDataUpdatePacket>(rawPacket);
            this.server.UpdateOrAddServer(packet);
            //Console.WriteLine("Server data update: code: {0} online: {1}  max:{2}", packet.ServerCode, packet.UserCount, packet.MaxUserCount);
        }
    }

    public struct ServerDataUpdatePacket : IPacket
    {
        public ShortPlainPacketHeader Head { get; set; }
        public short ServerCode { set; get; }
        public byte Percent { set; get; }
        public byte PlayType { set; get; }
        public short UserCount { set; get; }
        public short AccountCount { set; get; }
        public short MaxUserCount { set; get; }
    }
}
