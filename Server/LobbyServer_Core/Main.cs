﻿using CommonLib_Core;
using NetLib;
using NetLib.Token;
using Packet.Login;
using PacketLib_Core;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
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

        public static NetToken client;

        public static void OnAccept_CallBack(NetToken nettoken)
        {

        }

        public static void OnReceive_CallBack(INetSession netSession, short id, byte[] buffer)
        {

        }

        public static void OnConnect_CallBack(SocketError socketState, NetToken netToken)
        {

        }

        public static void OnDisConnect_CallBack(NetToken netToken)
        {

        }

        public class PacketReceiveHandler
        {
            public static void OnReceiveHandler(INetSession user, CS_Packet_Login packet)
            {

                SC_Packet_Login sc = new SC_Packet_Login();
                sc.UserID = packet.Name;
                user.SendPacket(sc);

                tempLogger.Debug($"[Server] Receive CS_Packet_Login {packet.Name}");
            }
        }

        static volatile bool exit = false;
        static ConsoleLogger tempLogger;

        static void Main(string[] args)
        {
            tempLogger = new ConsoleLogger();

            NetworkCore netWork = new NetworkCore(NET_TYPE.Server, tempLogger);
            netWork.Init_Server(3,
                OnAccept_CallBack,
                OnConnect_CallBack,
                OnReceive_CallBack,
                OnDisConnect_CallBack);
            netWork.Init_PacketHandler(Assembly.GetExecutingAssembly(), nameof(PacketReceiveHandler));

            Console.WriteLine("Start Server");
            netWork.Start_Server(8080, 100);


            while (!exit)
            {
                netWork.Update(0);
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
