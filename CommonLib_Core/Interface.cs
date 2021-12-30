using System;

namespace CommonLib_Core
{
    public interface IUser
    {
        string GetUserID();
    }

    public interface INetSession
    {
        IUser GetUser();
        void Close();
    }

    public interface ILogger
    {
        void Debug(string logMessage, params object[] logParams);

        void Info(string logMessage, params object[] logParams);

        void Warn(string logMessage, params object[] logParams);

        void Error(string logMessage, params object[] logParams);

        void Fatal(string logMessage, params object[] logParams);

        void Exception(string logMessage, System.Exception exceptionData);
    }
}
