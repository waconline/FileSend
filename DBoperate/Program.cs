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
using System.Collections.Generic;
using System.Data;
using System.ServiceModel;
using Oracle.ManagedDataAccess.Client;


namespace DBoperate
{
   

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
            using (var host = new ServiceHost(typeof(DataBaseService)))
            {
                
            }
        }
    }
}