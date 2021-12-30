using CommonLib_Core;
using NetLib;
using NetLib.Token;
using Packet.Login;
using System;
using System.Net.Sockets;

namespace ClientTool
{
    class Program
    {
        //테스트용 임시 로거
        public class ConsoleLogger : ILogger
        {
            public void Debug(string logMessage, params object[] logParams)
            {
                Console.WriteLine($"Debug : {logMessage}, param : {logParams}");
            }

            public void Error(string logMessage, params object[] logParams)
            {
                Console.WriteLine($"Error : {logMessage}, param : {logParams}");
            }

            public void Exception(string logMessage, Exception exceptionData)
            {
                Console.WriteLine($"Exception : {logMessage}, param : {exceptionData.ToString()}");
            }

            public void Fatal(string logMessage, params object[] logParams)
            {
                Console.WriteLine($"Fatal : {logMessage}, param : {logParams}");
            }

            public void Info(string logMessage, params object[] logParams)
            {
                Console.WriteLine($"Info : {logMessage}, param : {logParams}");
            }

            public void Warn(string logMessage, params object[] logParams)
            {
                Console.WriteLine($"Warn : {logMessage}, param : {logParams}");
            }
        }


        public static void OnReceive_CallBack(IUserToken userToken, byte[] buffer)
        {

        }

        public static void OnConnect_CallBack(SocketError socketState, NetToken netToken)
        {

        }

        public static void OnDisConnect_CallBack(NetToken netToken)
        {

        }

        static void Main(string[] args)
        {
            ConsoleLogger tempLogger = new ConsoleLogger();
            NetworkCore netWork = new NetworkCore(NET_TYPE.Server, tempLogger);
            netWork.Init_Client(OnConnect_CallBack, OnReceive_CallBack, OnDisConnect_CallBack);

            Console.WriteLine("Start Client");
            netWork.Start_Client("127.0.0.1", 8080, true);

            byte[] b = new byte[1];
            b[0] = 1;

            byte cnt = 1;

            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.KeyChar == 'H')
                {
                    byte[] nb = new byte[cnt];                                        
                    nb[cnt - 1] = cnt;
                    cnt++;

                    if (cnt == 3)
                    {
                        cnt -= 2;
                    }

                    netWork.SendPacket(nb);
                }
            }

            Console.WriteLine("Hello World!");
        }

    }
}
