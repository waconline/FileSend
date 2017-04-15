#region file header
// Username []
// Sln [FileSend]
// Project [Diploma]
// Filename [MyFTP.cs]
// Created  [12/04/2017 at 10:51]
// Clean up [14/04/2017 at 22:49]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"
#endregion

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Diploma
{
    public enum RequestsEnum : long
    {
        File_req,
        Info_req,
        File_info_req
    }

    public enum ResponsEnum : long
    {
        Ready,
        File_fnd,
        File_nfnd,
        Wrong_req
    }

    public class ReqestException : Exception
    {
        public new string Message;
        public string Request;

        public ReqestException(string msg, RequestsEnum req)
        {
            Message = msg;
            Request = req.ToString();
        }
    }

    public static class MyFTP
    {
        private const int FILE_CHUNK_SIZE = 1024;

        /// <summary>
        ///     Send request through socket and return respons code
        /// </summary>
        /// <param name="request">request which will be sended through socket</param>
        /// <param name="socket">socket with respons server</param>
        /// <returns>return <long> code of respons </returns>
        private static ResponsEnum Request(RequestsEnum request, Socket socket)
        {
            try
            {
                socket.Send(BitConverter.GetBytes((long) request));
                byte[] responsBytes = new byte[8];
                int recived = socket.Receive(responsBytes);
                long respons = BitConverter.ToInt64(responsBytes, 0);
                if ((ResponsEnum)respons == ResponsEnum.Wrong_req) throw new ReqestException("Wrong request", request);
                
                return (ResponsEnum)respons;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        ///     Send request for file on respons server.
        ///     If server return code of ready, then the file size will be received.
        /// </summary>
        /// <param name="filename">filename</param>
        /// <param name="socket">socket</param>
        /// <param name="fileSize">file size</param>
        /// <returns></returns>
        private static bool RequestFile(string filename, Socket socket, out long fileSize)
        {
            fileSize = 0;
            try
            {
                byte[] recivedBytes = new byte[8]; // long
                if (Request(RequestsEnum.File_req, socket) != ResponsEnum.Ready) return false;
                socket.Send(Encoding.ASCII.GetBytes(filename));
                int recived = socket.Receive(recivedBytes);

                //Console.WriteLine("recived resp {0}", BitConverter.ToInt64(recivedBytes, 0));
                if (recived < 8 || (ResponsEnum)BitConverter.ToInt64(recivedBytes, 0) != ResponsEnum.File_fnd) return false;
                recived = socket.Receive(recivedBytes);
                fileSize = BitConverter.ToInt64(recivedBytes, 0);

                return true;
            }
            catch (ReqestException rq)
            {
                Console.WriteLine(rq.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// Запрашивает файл и начинает его скачивание
        /// </summary>
        /// <param name="filename">Имя файла, который нужно скачать</param>
        /// <param name="socket">Сокет </param>
        /// <returns></returns>
        public static bool DownloadFile(string filename, Socket socket)
        {
            long fileSize;

            if (!RequestFile(filename, socket, out fileSize)) return false;

            BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create));
            long totalRecived = 0;
            Thread thread = new Thread(() => PercentShow(fileSize, ref totalRecived));
            thread.Start();

            while (totalRecived != fileSize)
            {
                byte[] fileChunk = new byte[FILE_CHUNK_SIZE];
                int recived = socket.Receive(fileChunk);
                totalRecived += recived;
                //Console.WriteLine(totalRecived);
                writer.Write(fileChunk);
            }

            thread.Join();
            writer.Close();
            return true;
        }

        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        private static void PercentShow(long total, ref long cur)
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
                    if (cur == total) return;
                }
            }
            catch (Exception e)
            {
            }
        }

        public static void Talk(Socket socket)
        {
            byte[] requestBytes = new byte[8];
            socket.Receive(requestBytes);
            long request = BitConverter.ToInt64(requestBytes, 0);
            Respons((RequestsEnum)request, socket);
        }

        private static void Respons(RequestsEnum request, Socket socket)
        {
            switch (request)
            {
                case RequestsEnum.File_req:
                    ResponsOnFileReq(socket);
                    break;
            }
        }

        private static void FileTranslate(Socket socket, string filename)
        {
            Console.WriteLine("start translate {0} to {1}", filename, socket);
            FileInfo info = new FileInfo(filename);
            socket.Send(BitConverter.GetBytes(info.Length));
            BinaryReader binReader = new BinaryReader(File.Open(filename, FileMode.Open));
            byte[] fileChunk = new byte[FILE_CHUNK_SIZE];
            int totalSended = 0;
            while (binReader.BaseStream.Position != binReader.BaseStream.Length)
            {
                fileChunk = binReader.ReadBytes(FILE_CHUNK_SIZE);
                int sended = socket.Send(fileChunk);
                totalSended += sended;
            }
            binReader.Close();
            Console.WriteLine("complite translate {0} to {1}, sended: {2}", filename, socket, totalSended);
        }

        private static ResponsEnum FileSearch(string filename)
        {
            try
            {
                //Console.WriteLine(filename);
                FileInfo info = new FileInfo(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return  ResponsEnum.File_nfnd;
            }
            Console.WriteLine("{0} find", filename);
            return ResponsEnum.File_fnd;
        }

        private static ResponsEnum PrepToTranslateFile()
        {
            //Console.WriteLine("ready");
            return  ResponsEnum.Ready;
        }

        private static void ResponsOnFileReq(Socket socket)
        {
            ResponsEnum status;

            status = PrepToTranslateFile();
            socket.Send(BitConverter.GetBytes((long)status)); // ready to send file resp
            if (status != ResponsEnum.Ready) return;

            byte[] fileNameRaw = new byte[512];
            socket.Receive(fileNameRaw); //receive file name  
            string filename = Encoding.ASCII.GetString(fileNameRaw).TrimEnd('\0');

            status = FileSearch(filename);
            socket.Send(BitConverter.GetBytes((long)status)); //Found file resp
            if (status != ResponsEnum.File_fnd) return;

            FileTranslate(socket, filename); // translate file to client
        }
    }
}