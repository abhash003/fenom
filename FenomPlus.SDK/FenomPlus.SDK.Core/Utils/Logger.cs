using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FenomPlus.SDK.Core.Utils
{
    public class Logger
    {
        private ILogger m_Logger;
        private string m_Category;

        public Logger(string Category)
        {
            m_Category = Category;
        }

        private bool CheckLogger(LogLevel logLevel)
        {
            if (m_Logger == null)
            {
                if (LoggingManager.GetInstance.Factory != null)
                {
                    m_Logger = LoggingManager.GetInstance.Factory.CreateLogger(m_Category);
                }
            }

            if (m_Logger is null)
            {
                return false;
            }

            return m_Logger.IsEnabled(logLevel);
        }

        public void LogInformation(string Message, params object[] Args)
        {
            if (CheckLogger(LogLevel.Information))
            {
                m_Logger.LogInformation(Message, Args);
            }

            System.Diagnostics.Trace.WriteLine(Message);
        }

        public void LogDebug(string Message, params object[] Args)
        {
            if (CheckLogger(LogLevel.Debug))
            {
                m_Logger.LogDebug(Message, Args);
            }

            System.Diagnostics.Debug.WriteLine(Message);
        }

        public void LogTrace(string Message, params object[] Args)
        {
            if (CheckLogger(LogLevel.Trace))
            {
                m_Logger.LogTrace(Message, Args);
            }

            System.Diagnostics.Trace.WriteLine(Message);
        }

        public void LogWarning(string Message, params object[] Args)
        {
            if (CheckLogger(LogLevel.Warning))
            {
                m_Logger.LogWarning(Message, Args);
            }

            System.Diagnostics.Trace.WriteLine(Message);
        }

        public void LogError(string Message, params object[] Args)
        {
            if (CheckLogger(LogLevel.Error))
            {
                m_Logger.LogError(Message, Args);
            }

            System.Diagnostics.Trace.WriteLine(Message);
        }

        public void LogCritical(string Message, params object[] Args)
        {
            if (CheckLogger(LogLevel.Critical))
            {
                m_Logger.LogCritical(Message, Args);
            }

            System.Diagnostics.Trace.WriteLine(Message);
        }

        /// <summary>
        /// Logs a warning and in debug mode does a console.writeline() print out
        /// </summary>
        public void LogException(Exception exception)
        {
            if (!CheckLogger(LogLevel.Warning))
            {
                Console.WriteLine("Logger.LogException() - not given high enough log level.");
                return;
            }

            LogWarning(exception.Message, exception);
#if DEBUG
            var formattedMessage = FormatException(exception);
            System.Diagnostics.Trace.WriteLine(formattedMessage);
            //Console.WriteLine(formattedMessage);
#endif
        }

        /// <summary>
        /// Attempts to format an exception as "Exception: stackTrace.exception"
        /// </summary>
        private string FormatException(Exception exception)
        {
            if (exception is null)
            {
                return "";
            }

            var stackTrace = exception.TargetSite?.DeclaringType?.ToString();

            var result = new StringBuilder();

            result.Append("Exception: ");

            if (stackTrace.IsNotEmpty())
            {
                result.Append(stackTrace);
            }

            if (exception.Message.IsNotEmpty())
            {
                result.Append(".");
                result.Append(exception.Message);
            }

            if (exception.InnerException != null && exception.InnerException.Message.IsNotEmpty())
            {
                result.Append(".");
                result.Append(exception.InnerException.Message);
            }

            return result.ToString();
        }
    }
}
