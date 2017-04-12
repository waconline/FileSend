#region file header

// Username []
// Sln [FileSend]
// Project [FileSendClient]
// Filename [MyFTP.cs]
// Created  [12/04/2017 at 10:23]
// Clean up [12/04/2017 at 10:27]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace FileSendClient
{
    public class MyFTP
    {
        private const int FILE_CHUNK_SIZE = 1024;

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
                byte[] fileChunk = new byte[FILE_CHUNK_SIZE];
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
    }
}