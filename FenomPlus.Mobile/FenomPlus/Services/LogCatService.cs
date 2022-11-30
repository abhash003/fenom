using System;
using FenomPlus.Interfaces;

namespace FenomPlus.Services
{
    public class LogCatService : BaseService, ILogCatService
    {
        public LogCatService(IAppServices services) : base(services)
        {
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public void Print(string msg, string exception = "")
        {
            Console.WriteLine(msg, exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        public void Print(Exception ex)
        {
            Print(ex.ToString());
        }
    }
}
