/// adb -d shell "run-as com.caireinc.fenomplus ls -l /data/data/com.caireinc.fenomplus/files/.local/share/*"
/// adb -d shell "run-as com.caireinc.fenomplus cat /data/data/com.caireinc.fenomplus/files/.local/share/debug_6222022.txt"
/// adb -d shell "run-as com.caireinc.fenomplus rm /data/data/com.caireinc.fenomplus/files/.local/share/debug_6222022.txt"
using System;
using System.IO;
using System.Threading.Tasks;
using FenomPlus.Interfaces;
using FenomPlus.Models;

namespace FenomPlus.Services
{
    public class DebugLogFileService : BaseService, IDebugLogFileService
    {        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public DebugLogFileService(IAppServices services) : base(services)
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetFilePath()
        {
            string LocalFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Services.LogCat.Print(LocalFolder);

            // make sure file
            string FileName = string.Format("debug_{0}.csv", DateTime.Now.ToShortDateString().Replace("/", "").Replace(",", "").Replace(" ", ""));
            Services.LogCat.Print(FileName);

            // Use Combine so that the correct file path slashes are used
            string filePath = Path.Combine(LocalFolder, FileName);

            return filePath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Write(DateTime dateTime, string msg)
        {
            if (dateTime == null) dateTime = DateTime.Now;
            string filePath = GetFilePath();
            string content = string.Format("{0},{1}\n", dateTime.ToString("yyyy/mm/dd HH:mm:ss:ffff"), msg);
            File.AppendAllText(filePath, content);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Write(DateTime dateTime, byte[] msg)
        {
            Write(dateTime, BitConverter.ToString(msg));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="debugLog"></param>
        public void Write(DebugLog debugLog)
        {
            Write(debugLog.DateTime, debugLog.Msg);
        }
    }
}
