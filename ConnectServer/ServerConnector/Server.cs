using ConnectServer.Packets.GameServerToClient;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ConnectServer.ServerConnector
{
    public class Server
    {
        public List<ServerObject> Servers { private set; get; } = new List<ServerObject>();

        public void Run(Config config)
        {
            this.receiveMessage(config.UdpPort);
        }
        public void UpdateOrAddServer(ServerDataUpdatePacket serverDataUpdatePacket)
        {
            foreach( ServerObject serverObject in this.Servers)
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
            lock (this.Servers)
            {
                this.Servers.Add(newServerObject);
            }
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
