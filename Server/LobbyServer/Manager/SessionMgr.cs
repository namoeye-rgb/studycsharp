using LobbyServer.Net;
using NetLib.Token;
using System.Collections.Generic;
using UtilLib;

namespace LobbyServer.Manager
{
    public class SessionMgr : Singleton<SessionMgr>
    {
        private Dictionary<ulong, ClientSession> sessions = new Dictionary<ulong, ClientSession>();

        public int SessionCount {
            get {
                return sessions.Count;
            }
        }

        public void CreateSession(NetToken _netToken)
        {
            lock (sessions) {
                var guid = GUIDGen.CreateGUID();

                var session = new ClientSession(_netToken, guid);
                _netToken.SetUserToken(session);
                sessions.Add(session.guid.Value, session);
            }
        }

        public void OnDisconnect(NetToken _netToken, out ClientSession session)
        {
            session = _netToken.GetUserToken() as ClientSession;
            if (session == null) {
                return;
            }

            lock (sessions) {
                if (sessions.ContainsKey(session.guid.Value) == false) {
                    return;
                }
                sessions.Remove(session.guid.Value);
            }
        }
    }
}
