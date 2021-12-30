using CommonLib_Core;
using NetLib;
using NetLib.Token;
using Packet.Login;
using System;
using System.Net.Sockets;
using System.Reflection;

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

        public static void OnConnect_CallBack(SocketError socketState, NetToken netToken)
        {
            netClient = netToken;
        }

        public static void OnDisConnect_CallBack(NetToken netToken)
        {

        }

        static NetToken netClient;

        public class PacketReceiveHandler
        {
            public static void OnReceiveHandler(INetSession user, SC_Packet_Login packet)
            {
                int b = 0;
            }
        }

        static void Main(string[] args)
        {
            ConsoleLogger tempLogger = new ConsoleLogger();
            NetworkCore netWork = new NetworkCore(NET_TYPE.Server, tempLogger);
            netWork.Init_Client(OnConnect_CallBack, OnReceive_CallBack, OnDisConnect_CallBack);
            netWork.Init_PacketHandler(Assembly.GetExecutingAssembly(), nameof(PacketReceiveHandler));
            Console.WriteLine("Start Client");
            netWork.Start_Client("127.0.0.1", 8080, true);



            while (true)
            {
                var keyInfo = Console.ReadKey();
                if (keyInfo.KeyChar == 'H')
                {
                    CS_Packet_Login t = new CS_Packet_Login();
                    t.LoginType = LoginType.Google;
                    t.Name = "testName";

                    netClient.SendPacket(t);
                }
            }

            Console.WriteLine("Hello World!");
        }

    }
}
