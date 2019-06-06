using ConnectServer.Packets;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ConnectServer
{
    public class Program
    {
        private static ClientConnector.Server clientServer;
        private static ServerConnector.Server udpServer;
        public static int Main()
        {
            Config config = new Config { Port = 44405, UdpPort = 55667 };

            Console.WriteLine("Starting Connect Server Udp on port: {0}", config.UdpPort);
            udpServer = new ServerConnector.Server();
            udpServer.Run(config);

            Console.WriteLine("Starting Connect Server on port: {0}", config.Port);
            clientServer = new ClientConnector.Server();
            clientServer.Run(config);

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