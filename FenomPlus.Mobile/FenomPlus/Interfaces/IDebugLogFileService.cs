using System;
using System.Threading.Tasks;
using FenomPlus.Helpers;
using FenomPlus.Models;

namespace FenomPlus.Interfaces
{
    public interface IDebugLogFileService
    {
        string GetFilePath();
        void Write(DateTime dateTime, string msg);
        void Write(DateTime dateTime, byte[] msg);
        void Write(DebugLog debugLog);
        void Write(RangeObservableCollection<DebugLog> debugList);
    }
}
