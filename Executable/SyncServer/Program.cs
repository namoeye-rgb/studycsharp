﻿using SyncServer.Net;
using System;

namespace SyncServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (Init() == false) {
                Console.ReadKey();
                return;
            }

            Run();
        }

        public static bool Init()
        {

            if (Server.Instance.Init() == false) {
                return false;
            }


            return true;
        }

        public static void Run()
        {
            Server.Instance.Update();
        }
    }
}
