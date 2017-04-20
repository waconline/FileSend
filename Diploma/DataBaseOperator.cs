#region file header

// Username []
// Sln [FileSend]
// Project [Diploma]
// Filename [DataBaseOperator.cs]
// Created  [20/04/2017 at 18:22]
// Clean up [20/04/2017 at 18:38]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace Diploma
{
    public class DataBaseOperator : IDisposable
    {
        // private string connectionString = "DATA SOURCE=XE;PASSWORD=4423;USER ID = SOUNDBASE";
        private readonly OracleConnection connection;

        public DataBaseOperator(string connectionString)
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

        public bool Registration(string Nickname, string Password, string Fname, string Sname, string Mail)
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
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool Login(string Nickname, string Password)
        {
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "LOGIN";

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
                        ParameterName = "P_PASSW",
                        Direction = ParameterDirection.Input,
                        OracleDbType = OracleDbType.Varchar2,
                        Value = Password
                    };
                    #endregion

                    command.Parameters.Add(userNicknameParameter);
                    command.Parameters.Add(userPasswordParameter);
                    command.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}