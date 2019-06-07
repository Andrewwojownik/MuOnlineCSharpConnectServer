using ConnectServer.Packets;
using ConnectServer.Packets.SC;
using ConnectServer.Packets.ServerClient;
using ConnectServer.ServerConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConnectServer.ClientConnector
{
    public class Server
    {
        private Config config;
        private ServerConnector.Server udpServer;
        public Server(Config config, ServerConnector.Server udpServer)
        {
            this.config = config;
            this.udpServer = udpServer;
        }
        public Task Run()
        {
            return Task.Run(async () =>
            {
                var tcpListener = TcpListener.Create(config.Port);
                tcpListener.Start();
                while (true)
                {
                    var tcpClient = await tcpListener.AcceptTcpClientAsync();
                    Console.WriteLine("[Server] Client has connected");

                    var task = HandleConnectionAsync(tcpClient);
                    // if already faulted, re-throw any error on the calling context
                    if (task.IsFaulted)
                        task.Wait();
                }
            });
        }

        private Task HandleConnectionAsync(TcpClient tcpClient)
        {
            WelcomePacket packet = new WelcomePacket();
            byte[] packetBytes = packet.CreatePacket();
            Send(tcpClient, packetBytes);
            //await tcpClient.GetStream().WriteAsync(packetBytes, 0, packetBytes.Length);

            NewsTitlePacket packet2 = new NewsTitlePacket();
            byte[] packetBytes2 = packet2.CreatePacket();
            //await tcpClient.GetStream().WriteAsync(packetBytes2, 0, packetBytes2.Length);
            Send(tcpClient, packetBytes2);

            NewsContentPacket packet3 = new NewsContentPacket();
            byte[] packetBytes3 = packet3.CreatePacket();
            //await tcpClient.GetStream().WriteAsync(packetBytes3, 0, packetBytes3.Length);
            Send(tcpClient, packetBytes3);

            return Task.Run(async () =>
            {
                using (var networkStream = tcpClient.GetStream())
                {
                    while (true)
                    {
                        var buffer = new byte[4096];
                        Console.WriteLine("[Server] Reading from client");
                        var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);

                        int? size = null;
                        HeadCodeSc? headcode = null;
                        byte type = buffer[0];

                        if (type == 0xC1 || type == 0xC3)
                        {
                            size = buffer[1];
                            headcode = (HeadCodeSc)buffer[2];
                            Console.WriteLine("C1/C3 packet type");
                        }
                        else if (type == 0xC2 || type == 0xC4)
                        {
                            size = buffer[1] * 256;
                            size |= buffer[2];
                            headcode = (HeadCodeSc)buffer[3];
                            Console.WriteLine("C2/C4 packet type");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unknow packet type 0x{0:X}", type);
                            Console.ResetColor();
                        }

                        Console.WriteLine("Read {0} bytes from socket.",
                            byteCount);
                        Console.WriteLine("Packet type: 0x{0:X} headcode: 0x{1:X} size: 0x{2:X}", type, headcode, size);
                        Console.WriteLine(BitConverter.ToString(buffer));

                        try
                        {
                            if (headcode == HeadCodeSc.ConnectServerData)
                            {
                                if ((HeadCodeCs)buffer[3] == HeadCodeCs.ClientConnect)
                                {
                                    ServerListPacket packetData = new ServerListPacket(udpServer);
                                    byte[] packetBytes4 = packetData.CreatePacket();

                                    Send(tcpClient, packetBytes4);
                                }
                                else if ((HeadCodeCs)buffer[3] == HeadCodeCs.ServerSelect)
                                {
                                    ServerDataPacket packetData = new ServerDataPacket(udpServer, buffer);
                                    byte[] packetBytes4 = packetData.CreatePacket();

                                    Send(tcpClient, packetBytes4);
                                }
                            }
                        } catch(Exception)
                        {
                            
                        }
                    }
                    //await HandleConnectionAsync(tcpClient);
                }
            });
        }

        private void Send(TcpClient tcpClient, byte[] packet)
        {
            try
            {
                NetworkStream stream = tcpClient.GetStream();
                if (stream.CanWrite)
                {
                    stream.BeginWrite(packet, 0, packet.Length, HandleDatagramWritten, tcpClient);
                }
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void HandleDatagramWritten(IAsyncResult ar)
        {
            try
            {
                ((TcpClient)ar.AsyncState).GetStream().EndWrite(ar);
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /*public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                // Signal the main thread to continue.  
                AllDone.Set();

                // Get the socket that handles the client request.  
                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.  
                StateObject state = new StateObject { WorkSocket = handler };
                handler.BeginReceive(state.Buffer, 0,
                    StateObject.BufferSize, 0, ReadCallback, state);

                WelcomePacket packet = new WelcomePacket();
                Send(handler, packet.CreatePacket());

                NewsTitlePacket packet2 = new NewsTitlePacket();
                Send(handler, packet2.CreatePacket());

                NewsContentPacket packet3 = new NewsContentPacket();
                Send(handler, packet3.CreatePacket());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }*/

        /*public static void ReadCallback(IAsyncResult ar)
        {
            // Retrieve the state object and the handler socket  
            // from the asynchronous state object.  
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.WorkSocket;

            // Read data from the client socket.   
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead == 0)
            {
                handler.Disconnect(false);
                return;
            }

            int? size = null;
            HeadCodeSc? headcode = null;
            byte type = state.Buffer[0];

            if (type == 0xC1 || type == 0xC3)
            {
                size = state.Buffer[1];
                headcode = (HeadCodeSc)state.Buffer[2];
                Console.WriteLine("C1/C3 packet type");
            }
            else if (type == 0xC2 || type == 0xC4)
            {
                size = state.Buffer[1] * 256;
                size |= state.Buffer[2];
                headcode = (HeadCodeSc)state.Buffer[3];
                Console.WriteLine("C2/C4 packet type");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unknow packet type 0x{0:X}", type);
                Console.ResetColor();
            }

            Console.WriteLine("Read {0} bytes from socket.",
                state.Buffer.Length);
            Console.WriteLine("Packet type: 0x{0:X} headcode: 0x{1:X} size: 0x{2:X}", type, headcode, size);
            Console.WriteLine(BitConverter.ToString(state.Buffer));

            if (headcode == HeadCodeSc.ConnectServerData)
            {
                if ((HeadCodeCs)state.Buffer[3] == HeadCodeCs.ClientConnect)
                {
                    ServerListPacket packetData = new ServerListPacket();
                    Send(handler, packetData.CreatePacket());
                }
                else if ((HeadCodeCs)state.Buffer[3] == HeadCodeCs.ServerSelect)
                {
                    byte[] packetData2 = { 0xC1, 0x15, 0xF4, 0x03, 0x31, 0x39, 0x32, 0x2e, 0x31, 0x36, 0x38, 0x2e, 0x31, 0x35, 0x38, 0x00, 0x00, 0x00, 0x00, 0x00, 0xDA, 0x5C };
                    Send(handler, packetData2);
                }
            }

            handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0,
                ReadCallback, state);
        }*/
    }
}
