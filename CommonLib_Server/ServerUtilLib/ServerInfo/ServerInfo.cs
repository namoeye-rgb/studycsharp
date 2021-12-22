using NetLib.Define;
using System;
using System.Collections.Generic;
using UtilLib.Data;

namespace NetLib
{
    public class ServerInfo
    {
        public SERVER_STATE ServerState { get; set; }
        public ushort ServerId { get; set; }

        public ServerConfig Config { get; set; }

        public ServerInfo()
        {
            ServerState = SERVER_STATE.NONE;
            ServerId = 0;
            Config = new ServerConfig();
        }

        public bool Init(string _jsonFileName)
        {
            try {
                Config = JsonLoadder.GetLoadJsonObject<ServerConfig>(_jsonFileName);
                return true;
            } catch (Exception e) {
                Console.WriteLine($"ServerInfo Error, {e}");
                return false;
            }
        }
    }

    public class Connection
    {
        public SERVER_TYPE ServerType { get; set; }
        public string Ip { get; set; }
        public ushort Port { get; set; }
    }

    public class ListenInfo
    {
        public ushort Port { get; set; }
        public int MaxConnection { get; set; }
    }

    public class DBInfo
    {
        public string ConnectionUrl { get; set; }
        public int Port { get; set; }
    }

    public class ServerConfig
    {
        public SERVER_TYPE ServerType { get; set; }
        public ushort Port { get; set; }
        public int MaxConnection { get; set; }
        public List<Connection> ConnectionInfo { get; set; }
        public ListenInfo ListenInfo { get; set; }
        public DBInfo DBInfo { get; set; }

        public ServerConfig()
        {
            ServerType = SERVER_TYPE.NONE;
            ConnectionInfo = new List<Connection>();
            Port = 5000;
            MaxConnection = 5000;
            ListenInfo = new ListenInfo();
            DBInfo = new DBInfo();
        }
    }
}
