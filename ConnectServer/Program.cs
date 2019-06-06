using ConnectServer.Packets;
using ConnectServer.Packets.SC;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ConnectServer
{
    public class Program
    {
        // Thread signal.  
        public static ManualResetEvent AllDone = new ManualResetEvent(false);

        public static void StartListening(Config config)
        {
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPAddress ipAddress = IPAddress.Parse("192.168.0.158"); // = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, config.Port);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    // Set the event to nonsignaled state.  
                    AllDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(AcceptCallback, listener);

                    // Wait until a connection is made before continuing.  
                    AllDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
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
        }

        public static void ReadCallback(IAsyncResult ar)
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
        }

        private static void Send(Socket handler, byte[] data)
        {
            Console.WriteLine(BitConverter.ToString(data));

            // Begin sending the data to the remote device.  
            handler.BeginSend(data, 0, data.Length, 0,
                SendCallback, handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                //handler.Shutdown(SocketShutdown.Both);
                //handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static int Main()
        {
            Config config = new Config { Port = 44405, UdpPort = 55667 };

            Console.WriteLine("Starting Connect Server Udp on port: {0}", config.UdpPort);
            ServerConnector.Server udpServer = new ServerConnector.Server();
            udpServer.Run(config);

            Console.WriteLine("Starting Connect Server on port: {0}", config.Port);
            Thread thread = new Thread(() => StartListening(config));
                     
            while (true)
            {
                ConsoleKeyInfo result = Console.ReadKey();
                if ((result.KeyChar == 'Q') || (result.KeyChar == 'q'))
                {
                    Console.WriteLine();
                    Console.WriteLine("Quit from app.");
                    Thread.Sleep(1000);
                    return 0;
                }
            }
        }

        public static byte[] ConvertStringToBytes(string str, int size)
        {
            byte[] strBytes = new byte[size];
            Array.Clear(strBytes, 0, strBytes.Length);
            Array.Copy(Encoding.ASCII.GetBytes(str), 0,
                strBytes, 0, str.Length);

            return strBytes;
        }

        public static T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }

    public static class PacketExtensions
    {
        public static byte[] GetBytes(this IPacket str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        public static byte[] Combine(this byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}