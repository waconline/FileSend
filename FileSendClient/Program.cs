#region file header

// Username []
// Sln [FileSend]
// Project [FileSendClient]
// Filename [Program.cs]
// Created  [11/04/2017 at 15:28]
// Clean up [11/04/2017 at 22:45]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Diploma;

namespace FileSendClient
{
    public class ReqestException : Exception
    {
        public string Message;

        public ReqestException(string msg)
        {
            Message = msg;
        }
    }

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
                    if (toSendString.Equals("file"))
                    {
                        Console.WriteLine("Recive file");
                        bool status = MyFTP.DownloadFile("2.jpg", sender);
                        Console.WriteLine("File Recived");
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