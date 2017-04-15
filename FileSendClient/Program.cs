#region file header
// Username []
// Sln [FileSend]
// Project [FileSendClient]
// Filename [Program.cs]
// Created  [11/04/2017 at 15:28]
// Clean up [13/04/2017 at 18:36]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"
#endregion

using System;
using System.Net;
using System.Net.Sockets;
using Diploma;

namespace FileSendClient
{ 

    internal class Program
    {
        private static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 4423);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Hello,i am client");
            try
            {
                sender.Connect(remoteEndPoint);
                string toSendString = "";

                while (!toSendString.Equals("exit"))
                {
                    toSendString = Console.ReadLine();
                    if (toSendString.Contains("download"))
                    {
                        string filename = toSendString.Split(' ')[1];
                        Console.WriteLine("Downloading file");
                        bool status = MyFTP.DownloadFile(filename, sender);
                        if(status) Console.WriteLine("File downloaded");
                        else
                        {
                            Console.WriteLine("Download error");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}