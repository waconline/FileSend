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
using Diploma;
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
            List<SoundInfo> info;
            using (DataBaseOperator DBoperator = new DataBaseOperator("DATA SOURCE=XE;PASSWORD=4423;USER ID = SOUNDBASE"))
            {
                DBoperator.GetSoundList("WHERE SL_ID = SL_ID", out info);
            }
            foreach (SoundInfo rec in info)
            {
                Console.WriteLine(rec.sl_name);
            }
            Console.ReadLine();
        }
    }
}