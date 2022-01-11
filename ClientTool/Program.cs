using CommonLib_Core;
using NetLib;
using NetLib.Token;
//using Packet.Login;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

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


        public static void OnReceive_CallBack(INetSession session, short id, byte[] buffer)
        {

        }

        public static void OnConnect_CallBack(SocketError socketState, Session netToken)
        {
            netClient = netToken;
        }

        public static void OnDisConnect_CallBack(Session netToken)
        {

        }

        static Session netClient;


        public class PacketReceiveHandler
        {
            //public static void OnReceiveHandler(INetSession user, SC_Packet_Login packet)
            //{
            //    tempLogger.Debug($"Echo : {packet.UserID}");
            //}
        }

        static ConsoleLogger tempLogger;
        static int count = 0;

        public delegate object ObjectProvider();
        public delegate string StringProvider();


        static IEnumerable<int> CreateSimpleIterator()
        {
            yield return 10;
            for (int i = 0; i < 3; i++)
            {
                yield return i;
            }

            yield return 20;
        }

        public class AA
        {

            public int A;
        }

        static void Main(string[] args)
        {
            

            var player = new
            {
                Name = "abc",
                Score = 100,
            };


            AA a = new AA();

            var testAny = new
            {
                AAA = a.A,
            };
           

            IEnumerable<int> enumerable = CreateSimpleIterator();

            using (IEnumerator<int> enumerator = enumerable.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    int value = enumerator.Current;
                    Console.WriteLine(value);
                }
            }

            //foreach (var value in CreateSimpleIterator())
            //{
            //    Console.WriteLine(value);
            //}


            //StringProvider stringProvider = new StringProvider(() =>
            //{
            //    return "";
            //});

            //ObjectProvider objectProvider = new ObjectProvider(stringProvider);


            //tempLogger = new ConsoleLogger();
            //NetworkCore netWork = new NetworkCore(NET_TYPE.Server, tempLogger);
            //netWork.Init_Client(OnConnect_CallBack, OnReceive_CallBack, OnDisConnect_CallBack);
            //netWork.Init_PacketHandler(Assembly.GetExecutingAssembly(), nameof(PacketReceiveHandler));
            //tempLogger.Debug("Start Client");
            //netWork.Start_Client("127.0.0.1", 8080, true);

            //Thread.Sleep(5000);

            //while (true)
            //{
            //    //var keyInfo = Console.ReadKey();
            //    //if (keyInfo.KeyChar == 'H')
            //    Thread.Sleep(10);
            //    {
            //        CS_Packet_Login t = new CS_Packet_Login();
            //        t.LoginType = LoginType.Google;
            //        count++;
            //        t.Name = $"testName : {count}";

            //        netClient.SendPacket(t);
            //        tempLogger.Debug($"[Client] Send CS_Packet_Login {count}");
            //    }
            //}

            //tempLogger.Debug("Hello World!");
        }

    }
}
