using CommonLib_Core;
using NetLib;
using NetLib.Token;
using PacketLib_Core;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static NetLib.NetworkCore;

namespace LobbyServer_Core
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


        public static void OnAccept_CallBack(NetToken nettoken)
        {

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

        static volatile bool exit = false;

        static void Main(string[] args)
        {
            ConsoleLogger tempLogger = new ConsoleLogger();

            PacketHandler handler = new PacketHandler();
            handler.Initialize(tempLogger);

            
            //NetworkCore netWork = new NetworkCore(NET_TYPE.Server, tempLogger);
            //netWork.Init_Server(3,
            //    OnAccept_CallBack,
            //    OnConnect_CallBack,
            //    OnReceive_CallBack,
            //    OnDisConnect_CallBack);

            //Console.WriteLine("Start Server");
            //netWork.Start_Server(8080, 100);


            //while (!exit)
            //{
            //    netWork.Update(0);
            //    Task.Factory.StartNew(() =>
            //    {
            //        while (Console.ReadKey().Key != ConsoleKey.Q) ;
            //        exit = true;
            //    });

            //    if (exit)
            //    {
            //        Console.WriteLine("Server Out");
            //        break;
            //    }

            //    Thread.Sleep(1000);
            //}
        }
    }
}
