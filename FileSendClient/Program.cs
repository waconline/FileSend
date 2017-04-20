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
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Diploma;

namespace FileSendClient
{ 

    internal class Program
    {
        private const string HelpString =
@"Help
To download file type 'download <filename>'
To Registrate type 'reg <Nickname> <Password> <Firstname> <Secondname> <Mail>'";
        private static Socket sender;

        private static void Download(string arg)
        {
            string filename = arg.Split(' ')[1];
            Console.WriteLine("Downloading file");
            bool status = MyFTP.DownloadFile(filename, sender);
            Console.WriteLine(status ? "Download success" : "Download fail");
        }

        private static void Registration(string arg)
        {
            string[] regDataStrings = arg.Split(' ');
            bool status = MyFTP.Registration(regDataStrings[1], regDataStrings[2], regDataStrings[3], regDataStrings[4],
                regDataStrings[5],sender);
            Console.WriteLine(status ? "Registration success" : "Registration fail");
        }

        private static bool Login()
        {
            Console.WriteLine("Nickname:");
            string nickname = Console.ReadLine();
            Console.WriteLine("Password:");
            string password = Console.ReadLine();
            return MyFTP.Login(nickname, password, sender);
        }

        private static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 4423);
            sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            Console.WriteLine("Hello,i am client");
            try
            {
                sender.Connect(remoteEndPoint);
                while (!Login())
                {
                    Console.WriteLine("try again? Y/n");
                    string answer = Console.ReadLine();
                    if (answer.Equals("n"))
                    {
                        sender.Close();
                        Environment.Exit(0);
                    }
                }
                Console.WriteLine("Login succes");
                string toSendString = "";

                while (!toSendString.Equals("exit"))
                {
                    toSendString = Console.ReadLine();
                    switch (toSendString.Split(' ')[0])
                    {
                        case "download":
                            Download(toSendString);
                            break;
                        case "reg":
                            Registration(toSendString);
                            break;
                        default:
                            Console.WriteLine(HelpString);
                            break;
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