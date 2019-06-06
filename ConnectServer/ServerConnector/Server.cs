using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ConnectServer.ServerConnector
{
    class Server
    {
        public void Run(Config config)
        {
            this.receiveMessage(config.UdpPort);
        }
        private async void receiveMessage(ushort port)
        {
            using (var udpClient = new UdpClient(port))
            {
                while (true)
                {
                    var receivedResult = await udpClient.ReceiveAsync();
                    Console.WriteLine(BitConverter.ToString(receivedResult.Buffer));
                }
            }
        }
    }
}
