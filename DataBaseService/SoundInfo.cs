#region file header
// Username []
// Sln [FileSend]
// Project [Diploma]
// Filename [SoundInfo.cs]
// Created  [23/04/2017 at 16:57]
// Clean up [23/04/2017 at 16:57]
// "we are circle 9. we are not retarded 
//  what we lack in brains we have in brawn"
#endregion

using System;

namespace DataBaseService
{
    public class SoundInfo
    {
        public string sl_id { get; set; }
        public string sl_name { get; set; }
        public string sl_upload_date { get; set; }
        public string sl_uploader { get; set; }
        public string sl_category { get; set; }
        public string sl_hash { get; set; }

        public string GetSingleString()
        {
            return $"{sl_id}\t{sl_name}\t{sl_upload_date}\t{sl_uploader}\t{sl_category}\t{sl_hash}";
        }

        public SoundInfo() { }

        public SoundInfo(string singleString)
        {
            string[] strings = singleString.Split('\t');
            sl_id = strings[0];
            sl_name = strings[1];
            sl_upload_date = strings[2];
            sl_uploader = strings[3];
            sl_category = strings[4];
            sl_hash = strings[5];
        }
    }
}