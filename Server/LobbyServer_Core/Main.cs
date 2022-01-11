using CommonLib_Core;
using NetLib;
using NetLib.Token;
using Packet_Login;
using PacketLib_Core;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static NetLib.NetworkCore_Listener;

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

        public static void OnAccept_CallBack(Session nettoken)
        {

        }

        public static void OnReceive_CallBack(INetSession netSession, short id, byte[] buffer)
        {

        }

        public static void OnConnect_CallBack(SocketError socketState, Session netToken)
        {

        }

        public static void OnDisConnect_CallBack(Session netToken)
        {

        }

        public class GameSession : Session
        {
            public override void OnConnectedHandler(Session netToken)
            {
                throw new NotImplementedException();
            }

            public override void OnDisConnectHandler(Session netToken)
            {
                throw new NotImplementedException();
            }

            public override void OnRecvHandler(INetSession userToken, short id, byte[] buffer)
            {
                throw new NotImplementedException();
            }
        }

        public class PacketReceiveHandler
        {
            public static void OnReceiveHandler(INetSession user, CS_Packet_Login packet)
            {

                SC_Packet_Login sc = new SC_Packet_Login();
                user.SendPacket(sc);

                tempLogger.Debug($"[Server] Receive CS_Packet_Login {packet.Name}");
            }
        }

        static volatile bool exit = false;
        static ConsoleLogger tempLogger;

        static void Main(string[] args)
        {
            tempLogger = new ConsoleLogger();

            var netConfig = new NetworkConfig();
            netConfig.ListenPort = 8080;
            netConfig.BackLogCount = 100;

            NetworkCore_Listener listener = new NetworkCore_Listener();
            listener.Init(netConfig, tempLogger, Assembly.GetExecutingAssembly(), nameof(PacketReceiveHandler), () => { return new GameSession(); });
            listener.RegisterHandler(
                OnAccept_CallBack,
                OnReceive_CallBack,
                OnDisConnect_CallBack);
            listener.Start_Server();

            while (!exit)
            {
                Task.Factory.StartNew(() =>
                {
                    while (Console.ReadKey().Key != ConsoleKey.Q) ;
                    exit = true;
                });

                if (exit)
                {
                    Console.WriteLine("Server Out");
                    break;
                }

                Thread.Sleep(1000);
            }
        }
    }
}
