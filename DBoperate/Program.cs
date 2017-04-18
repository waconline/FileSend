#region file header

// Username []
// Sln [FileSend]
// Project [DBoperate]
// Filename [Program.cs]
// Created  [18/04/2017 at 10:52]
// Clean up [18/04/2017 at 21:50]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DBoperate
{
    public class DataBaseOperator : IDisposable
    {
        private const string connectionString = "DATA SOURCE=XE;PASSWORD=4423;USER ID = SOUNDBASE";
        private readonly OracleConnection connection;

        public DataBaseOperator()
        {
            connection = new OracleConnection(connectionString);
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }

        ~DataBaseOperator()
        {
            Dispose();
        }

        public void UpdateUserDownloads(string User)
        {
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "UPDATE_USR_DWNLD";

                OracleParameter userNicknameParameter = new OracleParameter
                {
                    ParameterName = "P_NICKNAME",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = User
                };
                command.Parameters.Add(userNicknameParameter);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateUserUploads(string User)
        {
            using (OracleCommand command = new OracleCommand())
            {
                command.Connection = connection;
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "UPDATE_USR_UPLD";

                OracleParameter userNicknameParameter = new OracleParameter
                {
                    ParameterName = "P_NICKNAME",
                    Direction = ParameterDirection.Input,
                    OracleDbType = OracleDbType.Varchar2,
                    Value = User
                };
                command.Parameters.Add(userNicknameParameter);
                command.ExecuteNonQuery();
            }
        }

        private int GetHash(string Nickname, string Password)
        {
            int HashCode = Nickname.GetHashCode();
            HashCode = (HashCode * 397) ^ "9U4L4Q1CLD279JY4NMVBKPMLEYM1R5".GetHashCode();
            HashCode = (HashCode * 397) ^ Password.GetHashCode();
            return HashCode;
        }

        public void Registration(string Nickname, string Password, string Fname, string Sname, string Mail)
        {
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "INSERT_USR";

                    #region parameters 

                    OracleParameter userNicknameParameter = new OracleParameter
                    {
                        ParameterName = "P_NICKNAME",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Nickname
                    };

                    OracleParameter userPasswordParameter = new OracleParameter
                    {
                        ParameterName = "P_PASSWORD",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Password
                    };

                    OracleParameter userFirstnameParameter = new OracleParameter
                    {
                        ParameterName = "P_FIRSTNAME",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Fname
                    };

                    OracleParameter userSecondnameParameter = new OracleParameter
                    {
                        ParameterName = "P_SECONDNAME",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Sname
                    };

                    OracleParameter userMailParameter = new OracleParameter
                    {
                        ParameterName = "P_MAIL",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Mail
                    };

                    #endregion

                    command.Parameters.Add(userNicknameParameter);
                    command.Parameters.Add(userPasswordParameter);
                    command.Parameters.Add(userFirstnameParameter);
                    command.Parameters.Add(userSecondnameParameter);
                    command.Parameters.Add(userMailParameter);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public bool Login(string Nickname, string Password)
        {
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = @"SELECT U_PASSW FROM USR WHERE U_NICKNAME = :outer_nickname";

                    #region parameters 

                    OracleParameter userNicknameParameter = new OracleParameter
                    {
                        ParameterName = "outer_nickname",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Nickname
                    };

                    #endregion

                    command.Parameters.Add(userNicknameParameter);
                    OracleDataReader reader = command.ExecuteReader();
                    reader.Read();
                    string passwFromBase = reader.GetString(0);
                    reader.Close();
                    return passwFromBase.Equals(Password);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }

    internal class Program
    {
        private static int GetHash(string Nickname, string Password)
        {
            int HashCode = Nickname.GetHashCode();
            HashCode = (HashCode * 397) ^ "9U4L4Q1CLD279JY4NMVBKPMLEYM1R5".GetHashCode();
            HashCode = (HashCode * 397) ^ Password.GetHashCode();
            return HashCode;
        }

        private static void Main(string[] args)
        {
            using (DataBaseOperator DBoperator = new DataBaseOperator())
            {
                string passHash = GetHash("ULTROwaka", "4432").ToString();
                //DBoperator.Registration("ULTROwaka",passHash,"Nikita","Bokov","click-95@mail.ru");
                DBoperator.UpdateUserDownloads("ULTROwaka");
                DBoperator.Login("ULTROwaka", passHash);
            }
        }
    }
}