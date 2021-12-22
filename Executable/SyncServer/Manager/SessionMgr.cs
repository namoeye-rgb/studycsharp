using NetLib.Define;
using NetLib.Token;
using PacketLib;
using SyncServer.Net.Session;
using System.Collections.Generic;
using UtilLib;

namespace SyncServer.Manager
{
    public class SessionMgr : Singleton<SessionMgr>
    {
        private Dictionary<SERVER_TYPE, Dictionary<ushort, ClientSession>> sessions = new Dictionary<SERVER_TYPE, Dictionary<ushort, ClientSession>>();
        private ushort sessionNumbering = 0;
        private object sessionObject = new object();

        public int SessionCount {
            get {
                return sessions.Count;
            }
        }

        public void CreateSession(NetToken _netToken)
        {
            lock (sessionObject) {
                sessionNumbering++;
            }

            var session = new ClientSession(_netToken, sessionNumbering);
            _netToken.SetUserToken(session);
        }

        public void AddSession(ClientSession _session, SERVER_TYPE _serverType)
        {
            if (sessions.ContainsKey(_serverType) == false) {
                sessions.Add(_serverType, new Dictionary<ushort, ClientSession>());
            }

            _session.ServerType = _serverType;

            var serverSessions = sessions[_serverType];
            serverSessions.Add(_session.SessionUID, _session);
        }

        public ClientSession GetSession(SERVER_TYPE _serverType, ushort _sessionId)
        {
            if (sessions.ContainsKey(_serverType) == false) {
                return null;
            }

            var serverSessions = sessions[_serverType];

            if (serverSessions.ContainsKey(_sessionId) == false) {
                return null;
            }

            return serverSessions[_sessionId];
        }

        public void BroadCastPacket<T>(SERVER_TYPE _serverType, ushort _serverId, T _packet) where T : PK_BASE
        {
            if (sessions.ContainsKey(_serverType) == false) {
                return;
            }

            var serverSessions = sessions[_serverType];

            if (serverSessions.ContainsKey(_serverId)) {
                var session = serverSessions[_serverId];
                session.SendPacket(_packet);
            }
        }

        public void OnDisconnect(NetToken _netToken)
        {
            var session = _netToken.GetUserToken() as ClientSession;
            if (sessions.ContainsKey(session.ServerType) == false) {
                return;
            }

            var sessionDict = sessions[session.ServerType];

            lock (sessionDict) {
                if (sessionDict.ContainsKey(session.SessionUID)) {
                    sessionDict.Remove(session.SessionUID);
                }
            }
        }
    }
}
