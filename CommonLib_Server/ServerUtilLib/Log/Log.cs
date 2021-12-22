using log4net;
using log4net.Config;
using NetLib;
using System;
using System.IO;
using System.Reflection;

namespace UtilLib.Log
{
    public interface ILogger
    {
        ILog Logger { get; }
    }

    public class Log : Singleton<Log>, ILogger, INetLogger
    {
        public ILog Logger {
            get {
                return log4Net;
            }
        }

        private ILog log4Net;

        private const string LoggerConfigName = "log4net.config";

        public bool Init(string _serverType)
        {
            try {
                GlobalContext.Properties["ServerType"] = _serverType;

                var entryAssembly = Assembly.GetEntryAssembly();
                //var path = System.Environment.CurrentDirectory + "\\..\\..\\";
                XmlConfigurator.Configure(LogManager.GetRepository(entryAssembly), new FileInfo(LoggerConfigName));

                log4Net = LogManager.GetLogger(entryAssembly, "Logger");

                return true;
            } catch (Exception e) {
                Console.WriteLine($"Logger Init Error, {e}");
                return false;
            }
        }

        public void Debug(string logMessage, params object[] logParams)
        {
            log4Net.DebugFormat(logMessage, logParams);
        }

        public void Info(string logMessage, params object[] logParams)
        {
            log4Net.InfoFormat(logMessage, logParams);
        }

        public void Warn(string logMessage, params object[] logParams)
        {
            log4Net.WarnFormat(logMessage, logParams);
        }

        public void Error(string logMessage, params object[] logParams)
        {
            log4Net.ErrorFormat(logMessage, logParams);
        }

        public void Fatal(string logMessage, params object[] logParams)
        {
            log4Net.FatalFormat(logMessage, logParams);
        }

        private const string exceptionMsgFormat = "{0}{1}Exception: {2}{1}{3}";

        public void Exception(string logMessage, Exception exceptionData)
        {
            Error(exceptionMsgFormat, logMessage, Environment.NewLine, exceptionData.Message, exceptionData.StackTrace);
        }
    }
}
