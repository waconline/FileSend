#region file header

// Username []
// Sln [FileSend]
// Project [FileSendServer]
// Filename [Program.cs]
// Created  [11/04/2017 at 15:28]
// Clean up [13/04/2017 at 18:36]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Diploma;

namespace FileSendServer
{
    internal class Program
    {
        private const int FILE_CHUNK_SIZE = 1024;

        private static void Main(string[] args)
        {
#pragma warning disable 618
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
#pragma warning restore 618
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 4423);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //BinaryReader binReader = new BinaryReader(File.Open("2.jpg",FileMode.Open));
            Console.WriteLine("Hello,i am server");
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                try
                {
                    while (true)
                    {
                        Console.WriteLine("Wait a connection");
                        string data = "";
                        Socket connectedClientSocket = listener.Accept();
                        while (!data.Equals("exit"))
                        {
                            Console.WriteLine("Wait a message");
                            MyFTP.Talk(connectedClientSocket);
                            Console.WriteLine(data);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.ReadKey();
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
        }
    }
}