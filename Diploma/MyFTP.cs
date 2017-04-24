#region file header

// Username []
// Sln [FileSend]
// Project [Diploma]
// Filename [MyFTP.cs]
// Created  [12/04/2017 at 10:51]
// Clean up [20/04/2017 at 18:37]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Diploma
{
    public enum RequestsEnum : long
    {
        Reg_req,
        Login_req,
        File_req,
        Info_req,
        File_info_req,
        Soundlist_req
    }

    public enum ResponseEnum : long
    {
        Ready,
        File_fnd,
        File_nfnd,
        Wrong_req,
        Login_suc,
        Login_fail,
        Reg_suc,
        Reg_fail
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
        private static ResponseEnum Request(RequestsEnum request, Socket socket)
        {
            try
            {
                socket.Send(BitConverter.GetBytes((long) request));
                byte[] responseBytes = new byte[8];
                int recived = socket.Receive(responseBytes);
                long response = BitConverter.ToInt64(responseBytes, 0);
                if ((ResponseEnum) response == ResponseEnum.Wrong_req)
                    throw new ReqestException("Wrong request", request);

                return (ResponseEnum) response;
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
                if (Request(RequestsEnum.File_req, socket) != ResponseEnum.Ready) return false;
                socket.Send(Encoding.ASCII.GetBytes(filename));
                int recived = socket.Receive(recivedBytes);

                //Console.WriteLine("recived resp {0}", BitConverter.ToInt64(recivedBytes, 0));
                if (recived < 8 || (ResponseEnum) BitConverter.ToInt64(recivedBytes, 0) != ResponseEnum.File_fnd)
                    return false;
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
        ///     Запрашивает файл и начинает его скачивание
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
            Response((RequestsEnum) request, socket);
        }

        private static void Response(RequestsEnum request, Socket socket)
        {
            switch (request)
            {
                case RequestsEnum.File_req:
                    ResponseOnFileReq(socket);
                    break;
                case RequestsEnum.Reg_req:
                    ResponseOnRegistration(socket);
                    break;
                case RequestsEnum.Login_req:
                    ResponseOnLogin(socket);
                    break;
                case RequestsEnum.Soundlist_req:
                    ResponseOnGetSoundList(socket);
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

        private static ResponseEnum FileSearch(string filename)
        {
            try
            {
                //Console.WriteLine(filename);
                FileInfo info = new FileInfo(filename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return ResponseEnum.File_nfnd;
            }
            Console.WriteLine("{0} find", filename);
            return ResponseEnum.File_fnd;
        }

        private static ResponseEnum PrepToTranslateFile()
        {
            //Console.WriteLine("ready");
            return ResponseEnum.Ready;
        }

        private static void ResponseOnFileReq(Socket socket)
        {
            ResponseEnum status;

            status = PrepToTranslateFile();
            socket.Send(BitConverter.GetBytes((long) status)); // ready to send file resp
            if (status != ResponseEnum.Ready) return;

            byte[] fileNameRaw = new byte[512];
            socket.Receive(fileNameRaw); //receive file name  
            string filename = Encoding.ASCII.GetString(fileNameRaw).TrimEnd('\0');

            status = FileSearch(filename);
            socket.Send(BitConverter.GetBytes((long) status)); //Found file resp
            if (status != ResponseEnum.File_fnd) return;

            FileTranslate(socket, filename); // translate file to client
        }

        private static int GetHash(string Nickname, string Password)
        {
            int HashCode = Nickname.GetHashCode();
            HashCode = (HashCode * 397) ^ "9U4L4Q1CLD279JY4NMVBKPMLEYM1R5".GetHashCode();
            HashCode = (HashCode * 397) ^ Password.GetHashCode();
            return HashCode;
        }

        public static bool Registration(string Nickname, string Passsword, string Fname, string Sname, string Mail,
            Socket socket)
        {
            string passwordHash = GetHash(Nickname, Passsword).ToString();
            string singleString = $"{Nickname},{passwordHash},{Fname},{Sname},{Mail}";
            socket.Send(BitConverter.GetBytes((long) RequestsEnum.Reg_req));
            socket.Send(Encoding.ASCII.GetBytes(singleString));
            byte[] responseBytes = new byte[8];
            int recived = socket.Receive(responseBytes);
            long respons = BitConverter.ToInt64(responseBytes, 0);
            return (ResponseEnum) respons == ResponseEnum.Reg_suc;
        }

        private static void ResponseOnRegistration(Socket socket)
        {
            byte[] resivedBytes = new byte[512];
            socket.Receive(resivedBytes);
            string singleString = Encoding.ASCII.GetString(resivedBytes).TrimEnd('\0');
            string[] regDataString = singleString.Split(',');
            bool regStatus;
            using (
                DataBaseOperator DBoperator = new DataBaseOperator("DATA SOURCE=XE;PASSWORD=4423;USER ID = SOUNDBASE"))
            {
                regStatus = DBoperator.Registration(regDataString[0], regDataString[1], regDataString[2],
                    regDataString[3], regDataString[4]);
            }
            socket.Send(regStatus
                            ? BitConverter.GetBytes((long) ResponseEnum.Reg_suc)
                            : BitConverter.GetBytes((long) ResponseEnum.Reg_fail));
        }

        public static bool Login(string Nickname, string Passsword, Socket socket)
        {
            string passwordHash = GetHash(Nickname, Passsword).ToString();
            string singleString = $"{Nickname},{passwordHash}";
            socket.Send(BitConverter.GetBytes((long)RequestsEnum.Login_req));
            socket.Send(Encoding.ASCII.GetBytes(singleString));
            byte[] responseBytes = new byte[8];
            int recived = socket.Receive(responseBytes);
            long respons = BitConverter.ToInt64(responseBytes, 0);
            return (ResponseEnum)respons == ResponseEnum.Login_suc;
        }

        private static void ResponseOnLogin(Socket socket)
        {
            byte[] resivedBytes = new byte[512];
            int recived = socket.Receive(resivedBytes);
            string singleString = Encoding.ASCII.GetString(resivedBytes).TrimEnd('\0');
            string[] loginDataString = singleString.Split(',');
            bool loginStatus;
            using (DataBaseOperator DBoperator = new DataBaseOperator("DATA SOURCE=XE;PASSWORD=4423;USER ID = SOUNDBASE"))
            {
                loginStatus = DBoperator.Login(loginDataString[0], loginDataString[1]);
            }
            socket.Send(loginStatus
                            ? BitConverter.GetBytes((long)ResponseEnum.Login_suc)
                            : BitConverter.GetBytes((long)ResponseEnum.Login_fail));
        }

        public static void GetSoundList(string filter, Socket socket, out List<SoundInfo> soundlist)
        {
            soundlist = new List<SoundInfo>();
            socket.Send(BitConverter.GetBytes((long)RequestsEnum.Soundlist_req));
            socket.Send(Encoding.ASCII.GetBytes(filter));

            byte[] countOfSounds = new byte[8];
            int recived = socket.Receive(countOfSounds);
            long curCount = 0;
            long count = BitConverter.ToInt64(countOfSounds, 0);
            Console.WriteLine($"need {count}");
            while (curCount != count)
            {
                byte[] rowBytes = new byte[1024];
                recived = socket.Receive(rowBytes);
                string rowSingleString  = Encoding.ASCII.GetString(rowBytes).TrimEnd('\0');
                soundlist.Add(new SoundInfo(rowSingleString));
                curCount++;
            }
        }

        private static void ResponseOnGetSoundList(Socket socket)
        {
            byte[] filterBytes = new byte[512];
            int recived = socket.Receive(filterBytes);
            string filter = Encoding.ASCII.GetString(filterBytes).TrimEnd('\0');
            using (DataBaseOperator DBoperator = new DataBaseOperator("DATA SOURCE=XE;PASSWORD=4423;USER ID = SOUNDBASE"))
            {
                socket.Send(BitConverter.GetBytes(DBoperator.GetCountOfSound(filter)));
                List<SoundInfo> soundlist = new List<SoundInfo>();
                DBoperator.GetSoundList(filter, out soundlist);
                foreach (SoundInfo row in soundlist)
                {
                    byte[] rowBytes = new byte[1024];
                    Array.Copy(Encoding.ASCII.GetBytes(row.GetSingleString()),rowBytes, Encoding.ASCII.GetBytes(row.GetSingleString()).Length);
                    socket.Send(rowBytes);
                }
            }
        }
    }
}