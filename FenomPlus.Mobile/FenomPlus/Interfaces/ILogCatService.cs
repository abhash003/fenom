using System;

namespace FenomPlus.Interfaces
{
    public interface ILogCatService
    {
        void Print(string msg, string exception = "");
        void Print(Exception ex);
    }
}
