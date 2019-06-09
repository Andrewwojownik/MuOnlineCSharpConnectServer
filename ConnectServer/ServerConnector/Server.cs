using ConnectServer.Packets.GameServerToClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;

namespace ConnectServer.ServerConnector
{
    public class Server
    {
        public List<ServerObject> Servers { private set; get; } = new List<ServerObject>();
        public void LoadServerConfig()
        {
            var filename = "ServerList.xml";
            var currentDirectory = Directory.GetCurrentDirectory();
            var serverListFilepath = Path.Combine(currentDirectory, filename);
            XElement serverList = null;
            try
            {
                serverList = XElement.Load($"{serverListFilepath}");
            } catch (FileNotFoundException)
            {
                Console.WriteLine("ServerList.xml not found!");
            }

            lock (this.Servers)
            {
                this.Servers.Clear();

                IEnumerable<XElement> servers = from item in serverList.Descendants("Server")
                                                        select item;
                foreach(var server in servers)
                {
                    Console.WriteLine("Add server from config Code: {0} Name: {1} Visible: {2}", server.Attribute("Code").Value, server.Attribute("Name").Value, server.Attribute("Visible").Value);
                    ServerObject serverObject = new ServerObject
                    {
                        ServerCode = (short)Convert.ToInt32(server.Attribute("Code").Value),
                        IP = server.Attribute("IP").Value,
                        Port = (short)Convert.ToInt32(server.Attribute("Port").Value),
                        Visible = Convert.ToInt32(server.Attribute("Visible").Value) == 1 ? true : false,
                        Name = server.Attribute("Name").Value,
                    };
                    this.Servers.Add(serverObject);
                }
            }
        }

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
                    serverObject.LastUpdateDate = DateTime.Now;
                    return;
                }
            }

            Console.WriteLine("Unknow server ID: {0} try to update data", serverDataUpdatePacket.ServerCode);
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

                    //Console.WriteLine(BitConverter.ToString(receivedResult.Buffer));
                }
            }
        }
    }
}
