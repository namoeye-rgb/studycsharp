using GameDataLib;
using LobbyServer.DB;
using LobbyServer.Manager;
using NetLib;
using NetLib.Define;
using NetLib.Token;
using System;
using System.Threading;
using UtilLib;
using UtilLib.Log;

namespace LobbyServer.Net
{
    public partial class Server : Singleton<Server>
    {
        private ServerInfo serverInfo = new ServerInfo();

        public bool Init_Config(string _confingFileName)
        {
            var result = serverInfo.Init(_confingFileName);
            if (result == false)
            {
                return result;
            }

            result = Log.Instance.Init(serverInfo.Config.ServerType.ToString());
            return result;
        }

        private bool Init_DBConfig()
        {
            //DBConfig
            if (MongoDBMgr.Instance.Init(serverInfo.Config.DBInfo.ConnectionUrl, serverInfo.Config.DBInfo.Port) == false)
            {
                Log.Instance.Error("DB Init Error");
                return false;
            }

            return true;
        }

        public bool Init_Packet()
        {
            try
            {
                Log.Instance.Debug("Server Init Start");

                ////Packet
                //PacketLib.ZeroFormatter.PacketListInitializer.Init();

                //packetHandler = new PacketHandler();
                //packetHandler.SetHandler(typeof(Packet.PacketHandler_CS), "PacketReceive", (x, c, m) =>
                //{
                //    if (x.Length < 2)
                //    {
                //        return;
                //    }

                //    var pksId = PacketList.Get(x[1].ParameterType);
                //    if (pksId == 0)
                //    {
                //        return;
                //    }

                //    c.Add(pksId, m);
                //});

                //PacketParser.InitParseMethod();

                serverInfo.ServerState = SERVER_STATE.RUNNING;

                Log.Instance.Debug("Server Init End");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error($"DB Init Error, {e}");
                return false;
            }
        }

        public bool Init_Server()
        {
            try
            {
                Log.Instance.Debug($"Init NetTcpServer, Port : {serverInfo.Config.Port}");

                server = new TcpNetwork_Server
                {
                    OnAccept = OnAccept,
                    //OnReceiveComplete = OnReceive,
                    OnDisConnect = OnDisConnect
                };
                server.ServerStart(serverInfo.Config.Port, serverInfo.Config.MaxConnection);

                GUIDGen.Init(serverInfo.ServerId);

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error($"Init fail NetTcpServer, {e}");
                return false;
            }
        }

        public bool Init_S2SClient()
        {
            try
            {

                //var serverConnectionInfo = serverInfo.Config.ConnectionInfo.Find(x => x.ServerType == SERVER_TYPE.SYNC);

                //Log.Instance.Debug($"Init S2SClient, IP :{serverConnectionInfo.Ip}, Port : {serverConnectionInfo.Port}");

                //S2SClient.Instance.Init();
                //S2SClient.Instance.Start_Client(serverConnectionInfo.Ip, serverConnectionInfo.Port);

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error($"Init fail NetTcpServer, {e}");
                return false;
            }
        }

        public bool Init_Data()
        {
            try
            {
                //var result = ShopMgr.Instance.Init();
                var dataPath = System.IO.Directory.GetCurrentDirectory() + @"\..\..\GameData";
                var result = GameDataLoader.Instance.Init(dataPath);

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Error($"Init fail NetTcpServer, {e}");
                return false;
            }
        }

        public bool Init()
        {
            try
            {
                bool initResult = true;

                initResult &= Init_Config("ServerConfig.json");
                //initResult &= Init_DBConfig();
                initResult &= Init_Packet();
                initResult &= Init_Server();
                initResult &= Init_S2SClient();
                initResult &= Init_Data();

                if (initResult)
                {
                    Log.Instance.Debug("Server is Start");
                }

                return initResult;
            }
            catch (Exception e)
            {
                Log.Instance.Exception("Server Init Fail", e);
                return false;
            }
        }
    }

}
