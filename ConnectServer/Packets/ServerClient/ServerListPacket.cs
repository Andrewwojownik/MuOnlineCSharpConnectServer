using System;
using System.Runtime.InteropServices;
using ConnectServer.ServerConnector;

namespace ConnectServer.Packets.SC
{
    internal class ServerListPacket : ICreatePacketHandler
    {
        ServerConnector.Server server;

        public ServerListPacket(Server server)
        {
            this.server = server;
        }
        public byte[] CreatePacket()
        {
            byte[] returnBytes = new byte[4096];

            LongPlainPacketHeader head = new LongPlainPacketHeader
            {
                Type = Type.LongPlain,
                HeadCode = HeadCodeSc.ConnectServerData,
                HeadSubCode = HeadSubCodeSc.ServerList
            };

            ushort count = 0;

            foreach (ServerObject server in server.Servers)
            {
                if(server.Visible == false)
                {
                    continue;
                }

                if(server.LastUpdateDate.AddSeconds(5) < DateTime.Now)
                {
                    continue;
                }

                ServerListEntryPart serverListEntryPart = new ServerListEntryPart()
                {
                    ServerCode = server.ServerCode,
                    Percent = server.Percent,
                    PlayType = server.PlayType,
                };
                System.Buffer.BlockCopy(serverListEntryPart.GetBytes(), 0, returnBytes, Marshal.SizeOf(typeof(ServerListEntryPart))*count, Marshal.SizeOf(typeof(ServerListEntryPart)));
                ++count;
            }

            head.SetSize((ushort)
                (Marshal.SizeOf(typeof(ScServerListPacket))
                + Marshal.SizeOf(typeof(ServerListEntryPart)) * count));

            ScServerListPacket packet = new ScServerListPacket { Head = head };
            packet.SetCount(count);

            return packet.GetBytes()
                .Combine(returnBytes);
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
        public short ServerCode { set; get; }
        public byte Percent { set; get; }
        public byte PlayType { set; get; }
    }
}
