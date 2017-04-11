#region file header

// Username []
// Sln [FileSend]
// Project [FileSendServer]
// Filename [Program.cs]
// Created  [11/04/2017 at 15:28]
// Clean up [11/04/2017 at 15:43]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FileSendServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#pragma warning disable 618
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
#pragma warning restore 618
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 4423);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                            byte[] bytes = new byte[1024];
                            int bytesRec = connectedClientSocket.Receive(bytes);
                            data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                            Console.WriteLine(data);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);                
            }
        }
    }
}