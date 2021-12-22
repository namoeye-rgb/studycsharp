using FluentScheduler;
using GameCommon.Enum;
using GameServer.Manager;
using GameServer.Net.Session;
using GameServer.Scheduler;
using NetLib;
using NetLib.Define;
using PacketLib;
using System;
using System.Diagnostics;
using System.Threading;
using UtilLib;
using UtilLib.Log;

namespace GameServer.Net
{
    public partial class Server : Singleton<Server>
    {
        public RakServer RakServer { get; set; }

        //private S2STcpServer s2sTcpServer = new S2STcpServer();

        public ServerInfo serverInfo = new ServerInfo();

        public PacketHandler packetHandler;

        private bool Init_Config()
        {
            //ConfigLoader
            //var path = System.Environment.CurrentDirectory + "\\..\\..\\";
            if (serverInfo.Init("ServerConfig.json") == false) {
                Log.Instance.Error("Server ConfigLoader Error");
                return false;
            }
            if (Log.Instance.Init(serverInfo.Config.ServerType.ToString()) == false) {
                return false;
            }

            return true;
        }

        private bool Init_Packet()
        {
            try
            {
                PacketLib.ZeroFormatter.PacketListInitializer.Init();
                PacketParser.InitParseMethod();
                packetHandler = new PacketHandler();
                packetHandler.SetHandler(typeof(Packet.PacketHandler_CG), "PacketReceive", (x, c, m) => {
                    if (x.Length < 2)
                    {
                        return;
                    }

                    var pksId = PacketList.Get(x[1].ParameterType);
                    if (pksId == 0)
                    {
                        return;
                    }

                    c.Add(pksId, m);
                });
                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error($"Fail Packet Init, {e}");
                return false;
            }
        }

        private bool Init_Server()
        {
            try {
                Log.Instance.Debug("Server Init Start");

                RakServer = new RakServer(Log.Instance);
                RakServer.OnNewConnection = OnNewConnection;
                RakServer.OnClosedConnection = OnClosedConnection;
                RakServer.OnReceived = OnReceive;

                RakServer.SetTimeoutTime(5000);
                RakServer.Startup("127.0.0.1", serverInfo.Config.Port, 5000);

                serverInfo.ServerState = SERVER_STATE.RUNNING;

                Log.Instance.Debug("Server Init End");

                return true;
            } catch (Exception e) {
                Log.Instance.Debug($"Fail Server Init Start, {e}");
                return false;
            }
        }

        public bool Init_S2SClient()
        {
            try {

                var serverConnectionInfo = serverInfo.Config.ConnectionInfo.Find(x => x.ServerType == SERVER_TYPE.SYNC);

                Log.Instance.Debug($"Init S2SClient, IP :{serverConnectionInfo.Ip}, Port : {serverConnectionInfo.Port}");

                S2SClient.Instance.Init();
                S2SClient.Instance.Start_Client(serverConnectionInfo.Ip, serverConnectionInfo.Port);

                return true;
            } catch (Exception e) {
                Log.Instance.Error($"Init fail NetTcpServer, {e}");
                return false;
            }
        }

        public bool Init_Scheduler()
        {
            try {
                JobManager.Initialize(new Schedule_BattleInfo());

                return true;
            } catch (Exception e) {
                Log.Instance.Error($"Init fail JobManager, {e}");
                return false;
            }
        }

        public bool Init()
        {
            try {
                bool initResult = true;

                initResult &= Init_Config();
                initResult &= Init_Packet();
                initResult &= Init_Server();
                initResult &= Init_S2SClient();
                initResult &= Init_Scheduler();

                return initResult;
            } catch (Exception e) {
                Log.Instance.Exception("Server Init Fail", e);
                return false;
            }
        }

        public void Close()
        {
            RakServer.ShutDown(2000);
        }

        public void Update()
        {
            Log.Instance.Debug("Server Run");

            var stopwatch = new Stopwatch();
            double lastTick = 0;

            while (true) {
                try {
                    stopwatch.Restart();

                    RakServer.Update();

                    Thread.Sleep(1);
                    lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;
                } catch (Exception e) {
                    Log.Instance.Exception("Server Run Fail", e);
                    break;
                }
            }

            Log.Instance.Debug("Server Stop and Close");

            Stop();
            Close();
        }

        public void Stop()
        {
            SessionMgr.Instance.Stop();
        }

        public void OnNewConnection(ulong _guid, bool _isIncoming)
        {
            var result = SessionMgr.Instance.CreateSession(_guid, RakServer);
            if (result != ERROR_CODE.SUCEESS) {
                Log.Instance.Debug(result.ToString());
                return;
            }
        }

        public void OnReceive(ulong _guid, byte[] _buffer)
        {
            var packetId = BitConverter.ToUInt16(_buffer, 1);
            var packetData = PacketParser.Parse(packetId, _buffer, PacketDefine.CS_RakNet_Packet_Head_Size);

            Log.Instance.Debug($"Packet Receive Id : {packetData.ToString()}");

            if (SessionMgr.Instance.GetSession(_guid, out ClientSession session) != ERROR_CODE.SUCEESS)
            {
                Log.Instance.Warn($"not found session : {_guid}");
                return;
            }

            packetHandler.DispatchPacket(packetId, session, packetData);
        }

        public void OnClosedConnection(ulong _guid, CLOSE_CONNECTION_TYPE _type)
        {
            ERROR_CODE result = ERROR_CODE.SUCEESS;

            result = SessionMgr.Instance.GetSession(_guid, out ClientSession session);

            if (result != ERROR_CODE.SUCEESS) {
                Log.Instance.Warn("Not Found ClientSession", _guid);
                return;
            }

            result = UserMgr.Instance.RemoveUser(session.UserUID);
            if (result != ERROR_CODE.SUCEESS)
            {
                Log.Instance.Warn("Fail RemoveUser", session.UserUID);
            }

            if (_type == CLOSE_CONNECTION_TYPE.DISCONNECTION_NOTIFICATION) {
                result = SessionMgr.Instance.ClosedConnection(session);
                if (result != ERROR_CODE.SUCEESS) {
                    Log.Instance.Warn("Fail ClosedConnection", _guid);
                }
            }

            result = SessionMgr.Instance.RemoveSession(session);
            if (result != ERROR_CODE.SUCEESS) {
                Log.Instance.Warn("Fail Session Remove", _guid);
            }

            Log.Instance.Debug($"OnClosedConection User, {_guid}");
        }
    }
}
