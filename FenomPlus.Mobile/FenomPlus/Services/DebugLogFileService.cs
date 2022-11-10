/// adb -d shell "run-as com.caireinc.fenomplus ls -l /data/data/com.caireinc.fenomplus/files/.local/share/*"
/// adb -d shell "run-as com.caireinc.fenomplus cat /data/data/com.caireinc.fenomplus/files/.local/share/debug_6222022.txt"
/// adb -d shell "run-as com.caireinc.fenomplus rm /data/data/com.caireinc.fenomplus/files/.local/share/debug_6222022.txt"
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FenomPlus.Interfaces;
using FenomPlus.Models;

namespace FenomPlus.Services
{
    public class DebugLogFileService : BaseService, IDebugLogFileService
    {
        public DebugLogFileService(IAppServices services) : base(services)
        {
            
        }

        public string GetFilePath()
        {
            string LocalFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Services.LogCat.Print(LocalFolder);

            // make sure file
            string FileName =
                $"debug_{DateTime.Now.ToShortDateString().Replace("/", "").Replace(",", "").Replace(" ", "")}.csv";
            Services.LogCat.Print(FileName);

            // Use Combine so that the correct file path slashes are used
            string filePath = Path.Combine(LocalFolder, FileName);

            return filePath;
        }

        public void Write(DateTime dateTime, string msg)
        {
            if (dateTime == null) 
                dateTime = DateTime.Now;

            string filePath = GetFilePath();
            string content = $"{dateTime.ToString(Constants.DateTimeFormatString, CultureInfo.InvariantCulture)},{msg}\n";
            File.AppendAllText(filePath, content);
        }

        public void Write(DateTime dateTime, byte[] msg)
        {
            Write(dateTime, BitConverter.ToString(msg));
        }

        public void Write(DebugLog debugLog)
        {
            Write(debugLog.DateTime, debugLog.Msg);
        }
    }
}
