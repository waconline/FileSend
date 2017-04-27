#region file header

// Username []
// Sln [FileSend]
// Project [DataBaseService]
// Filename [DataBaseOperator.cs]
// Created  [26/04/2017 at 22:55]
// Clean up [27/04/2017 at 16:50]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace DataBaseService
{
    public class DataBaseOperator : IDisposable, IDataBaseOperator
    {
        private static readonly string connectionString = "DATA SOURCE=XE;PASSWORD=4423;USER ID=SOUNDBASE";
        private readonly OracleConnection connection = new OracleConnection(connectionString);

        public DataBaseOperator()
        {
        }

        public DataBaseOperator(string connectionString)
        {
            connection = new OracleConnection(connectionString);
            connection.Open();
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
                connection.Open();
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
            finally
            {
                connection.Close();
            }
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

        public long GetCountOfSound(string filter)
        {
            long count = 0;
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT COUNT(*) FROM SOUNDLIST {filter}";

                    #region parameters 

                    #endregion

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                        count = reader.GetInt64(0);
                    }
                    return count;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }

        public bool GetSoundList(string filter, out List<SoundInfo> soundlist)
        {
            soundlist = new List<SoundInfo>();
            try
            {
                using (OracleCommand command = new OracleCommand())
                {
                    command.Connection = connection;
                    command.CommandType = CommandType.Text;
                    command.CommandText = $"SELECT * FROM SOUNDLIST {filter}";

                    #region parameters 

                    #endregion

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SoundInfo row = new SoundInfo
                            {
                                sl_id = reader.GetDecimal(0).ToString(),
                                sl_name = reader.GetString(1),
                                sl_upload_date = reader.GetDateTime(2).ToString(),
                                sl_uploader = reader.GetDecimal(3).ToString(),
                                sl_category = reader.GetString(4),
                                sl_hash = reader.GetString(6)
                            };
                            soundlist.Add(row);
                        }
                    }

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