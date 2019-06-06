using ConnectServer.Packets.GameServerToClient;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ConnectServer.ServerConnector
{
    class Server
    {
        List<ServerObject> servers = new List<ServerObject>();
        public void Run(Config config)
        {
            this.receiveMessage(config.UdpPort);
        }

        public void UpdateOrAddServer(ServerDataUpdatePacket serverDataUpdatePacket)
        {
            foreach( ServerObject serverObject in this.servers)
            {
                if(serverObject.ServerCode == serverDataUpdatePacket.ServerCode)
                {
                    serverObject.Percent = serverDataUpdatePacket.Percent;
                    return;
                }
            }

            ServerObject newServerObject = new ServerObject();
            newServerObject.ServerCode = serverDataUpdatePacket.ServerCode;
            newServerObject.Percent = serverDataUpdatePacket.Percent;
            this.servers.Add(newServerObject);
        }
        private async void receiveMessage(ushort port)
        {
            using (var udpClient = new UdpClient(port))
            {
                while (true)
                {
                    var receivedResult = await udpClient.ReceiveAsync();

                    ServerUpdatePacket packet = new ServerUpdatePacket(this);
                    packet.Handle(receivedResult.Buffer);

                    Console.WriteLine(BitConverter.ToString(receivedResult.Buffer));
                }
            }
        }
    }
}
