using NetLib;
using NetLib.Define;
using NetLib.Token;
using PacketLib;
using SyncServer.Manager;
using System;
using System.Diagnostics;
using System.Threading;
using UtilLib;
using UtilLib.Log;

namespace SyncServer.Net
{
    public partial class Server : Singleton<Server>
    {
        private TcpNetwork_Server server;

        public PacketHandler packetHandler;

        public ServerInfo serverInfo = new ServerInfo();

        public bool Init_Config()
        {
            if (serverInfo.Init("ServerConfig.json") == false) {
                Log.Instance.Error("Server ConfigLoader Error");
                return false;
            }
            if (Log.Instance.Init(serverInfo.Config.ServerType.ToString()) == false) {
                return false;
            }

            return true;
        }

        public bool Init_Packet()
        {
            try {
                Log.Instance.Debug("Server Init Start");

                //Packet
                PacketLib.ZeroFormatter.PacketListInitializer.Init();

                packetHandler = new PacketHandler();
                packetHandler.SetHandler(typeof(Packet.PacketHandler_S2S), "PacketReceive", (x, c, m) => {
                    if (x.Length < 2) {
                        return;
                    }

                    var pksId = PacketList.Get(x[1].ParameterType);
                    if (pksId == 0) {
                        return;
                    }

                    c.Add(pksId, m);
                });

                PacketParser.InitParseMethod();

                serverInfo.ServerState = SERVER_STATE.RUNNING;

                Log.Instance.Debug("Server Init End");

                return true;
            } catch (Exception e) {
                Log.Instance.Error($"DB Init Error, {e}");
                return false;
            }
        }

        public bool Init_Server()
        {
            try {
                Log.Instance.Debug($"Init NetTcpServer, Port : {serverInfo.Config.Port}");

                server = new TcpNetwork_Server();

                server.OnAccept = OnAccept;
                server.OnReceiveComplete = OnReceive;
                server.OnDisConnect = OnDisConnect;
                server.ServerStart(serverInfo.Config.Port, serverInfo.Config.MaxConnection);

                return true;
            } catch (Exception e) {
                Log.Instance.Error($"Init fail NetTcpServer, {e}");
                return false;
            }
        }

        public bool Init_Data()
        {
            try {
                //var path = System.IO.Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\..\\Data\\XML\\Battle\\BattleGrade.xml";
                //ESTableManager.Load(path);

                return true;
            } catch (Exception e) {
                Log.Instance.Exception("Server Init Fail", e);
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
                initResult &= Init_Data();

                if (initResult) {
                    Log.Instance.Debug("Server is Start");
                }

                return initResult;
            } catch (Exception e) {
                Log.Instance.Exception("Server Init Fail", e);
                return false;
            }
        }

        public void Update()
        {
            var stopwatch = new Stopwatch();
            double lastTick = 0;

            while (true) {
                stopwatch.Restart();
                Thread.Sleep(1);

                lastTick = (double)stopwatch.ElapsedTicks / Stopwatch.Frequency;

                server.Update();
            }
        }

        public void OnAccept(NetToken _acceptNetToken)
        {
            SessionMgr.Instance.CreateSession(_acceptNetToken);
            Log.Instance.Warn($"Client OnAccept");
        }
        public void OnReceive(IUserToken _userToken, ushort _packetId, PK_BASE _packet)
        {
            packetHandler?.DispatchPacket(_packetId, _userToken, _packet);
        }

        public void SendPacket<T>(T _packet) where T : PK_BASE
        {
            var packetBuffer = Serializer.Serialize_Server(_packet);
            //server.SendPacket(packetBuffer);
        }

        public void OnDisConnect(NetToken _disconnectToken)
        {
            SessionMgr.Instance.OnDisconnect(_disconnectToken);
        }
    }
}
