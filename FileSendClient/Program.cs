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
        public static string Request(string request, Socket socket)
        {
            try
            {
                socket.Send(Encoding.ASCII.GetBytes(request));
                byte[] responsBytes = new byte[512];
                int recived = socket.Receive(responsBytes);
                string respons = Encoding.ASCII.GetString(responsBytes);
                if (respons.Equals(":Wrong_req:")) throw new ReqestException("Wrong request");
                return respons;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static bool RequestFile(string filename, Socket socket, out long fileSize)
        {
            try
            {
                fileSize = 0;
                //string str = Request(":File_req:", socket);
                //Console.WriteLine(str);
                //bool status = str.Contains(":File_fnd:");
                //Console.WriteLine(status);
                if (!Request(":File_req:", socket).Contains(":File_fnd:")) return false;
                byte[] recivedBytes = new byte[8]; // long
                int recived = socket.Receive(recivedBytes);
                if (recived < 8) return false;
                fileSize = BitConverter.ToInt64(recivedBytes, 0);
                return true;
            }
            catch (ReqestException rq)
            {
                Console.WriteLine(rq.Message);
                fileSize = 0;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
       
        public static bool DownloadFile(string filename, Socket socket)
        {
            long fileSize;
            //bool status = RequestFile(filename, socket, out fileSize);
            //Console.WriteLine(status);
            if (!RequestFile(filename, socket, out fileSize)) return false;
            //Console.WriteLine(fileSize);
            BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            long totalRecived = 0;
            Thread thread = new Thread(() => PercentShow(fileSize, ref totalRecived));
            thread.Start();
            while (totalRecived != fileSize)
            {
                byte[] fileChunk = new byte[1];
                int recived = socket.Receive(fileChunk);
                totalRecived += recived;
                writer.Write(fileChunk);
            }
            thread.Abort();
            writer.Close();
            return true;
        }

        public static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public static void PercentShow(long total, ref long cur)
        {
            try
            {
                while (true)
                {
                    if (cur != 0)
                    {
                        Console.Write("{0}%", cur * 100 / total);
                        ClearCurrentConsoleLine();
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private static void Main(string[] args)
        {
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, 4423);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //BinaryWriter writer = new BinaryWriter(File.Open("recv.jpg",FileMode.Create));
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
                        bool status = DownloadFile("1.jpg", sender);
                        Console.WriteLine(status);
                        //byte[] filesize = new byte[8];
                        //int recived = sender.Receive(filesize);
                        //Console.WriteLine("file size {0} : {1}", BitConverter.ToInt64(filesize, 0), recived);
                        //int totalRevived = 0;
                        //while (totalRevived != BitConverter.ToInt32(filesize, 0))
                        //{
                        //    byte[] fileChunk = new byte[1];                           
                        //    recived = sender.Receive(fileChunk);
                        //    totalRevived += recived;
                        //    //Console.WriteLine(recived+" : "+totalRevived);
                        //    writer.Write(fileChunk);                     
                        //}
                        //writer.Close();
                        Console.WriteLine("File Recived");
                    }
                    //sender.Send(Encoding.ASCII.GetBytes(toSendString));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}