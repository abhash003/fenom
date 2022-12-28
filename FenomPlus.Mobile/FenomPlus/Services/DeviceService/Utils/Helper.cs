using FenomPlus.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;

namespace FenomPlus.Services.DeviceService.Utils
{
    public struct Helper
    {
        public static void WriteDebug(string msg, int prependCount = 0, char prependChar = ' ')
        {
#if DEBUG
            var logMessage = string.Format("{0} : [{1}] - {2}", new string(prependChar, prependCount), Thread.CurrentThread.ManagedThreadId, msg);

            var logger = AppServices.Container.Resolve<ILogCatService>();

            if (logger == null)
            {
                throw new ArgumentNullException("No logger available. Something is wrong!");
            }

            logger.Print(logMessage);
#endif
        }

        public static void WriteDebug(Exception ex)
        {
            WriteDebug($"Exception: {ex.Message} (${ex.ToString()})");
        }

        public class FunctionTrace : IDisposable
        {
            //private static int _indentLevel = 1;

            private readonly static string _pre = "--------> Entering: ";
            private readonly static string _post = "<-------- Exiting:  ";
            private readonly static char _indentChar = ' ';

            private readonly static int _skipFrames = 1;
            private string _functionName;

            public FunctionTrace()
            {
                _functionName = new StackFrame(_skipFrames).GetMethod().ReflectedType.FullName;
                Prologue();
            }

            public void Dispose()
            {
                Epilogue();
            }

            private void Prologue()
            {
                WriteDebug($"{_pre} {_functionName}");
            }
            private void Epilogue()
            {
                WriteDebug($"{_post} {_functionName}");
            }

            public void Trace(string msg)
            {
                var indent = 2;
                var spacer = $"{new string(_indentChar, indent)}";
                WriteDebug($"{spacer} {msg}");
            }
        }
    }
}
