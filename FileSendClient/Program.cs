#region file header

// Username []
// Sln [FileSend]
// Project [FileSendClient]
// Filename [Program.cs]
// Created  [11/04/2017 at 15:28]
// Clean up [11/04/2017 at 15:48]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
            BinaryWriter writer = new BinaryWriter(File.Open("recv.jpg",FileMode.Create));
            Console.WriteLine("Hello,i am client");
            try
            {
                sender.Connect(remoteEndPoint);
                string toSendString = "";

                while (!toSendString.Equals("exit"))
                {
                    toSendString = Console.ReadLine();                    
                    sender.Send(Encoding.ASCII.GetBytes(toSendString));
                    if (toSendString.Equals("file"))
                    {
                        
                        Console.WriteLine("Recive file");
                        byte[] filesize = new byte[4];
                        int recived = sender.Receive(filesize);
                        Console.WriteLine("file size {0} : {1}", BitConverter.ToInt32(filesize, 0), recived);
                        int totalRevived = 0;
                        while (totalRevived == BitConverter.ToInt32(filesize, 0))
                        {
                            byte[] fileChunk = new byte[1];                           
                            recived = sender.Receive(fileChunk);
                            totalRevived += recived;
                            Console.WriteLine(recived+" : "+totalRevived);
                            writer.Write(fileChunk);                     
                        }
                        writer.Close();
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