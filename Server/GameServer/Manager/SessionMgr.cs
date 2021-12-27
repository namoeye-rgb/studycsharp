using EntityService;
using GameCommon.Enum;
using GameServer.Net.Session;
using NetLib;
using PacketLib;
using System.Collections.Generic;
using UtilLib;

namespace GameServer.Manager
{
    public class SessionMgr : Singleton<SessionMgr>
    {
        private readonly Dictionary<ESHandle, ClientSession> m_sessions = new Dictionary<ESHandle, ClientSession>();

        public int SessionCount {
            get {
                return m_sessions.Count;
            }
        }

        public ERROR_CODE CreateSession(ulong _guid, RakServer _rakServer)
        {
            if (m_sessions.ContainsKey(_guid) == true) {
                return ERROR_CODE.PLAYER_ALREADY_EXISTS;
            }

            var _session = new ClientSession();
            _session.Initialize(_guid, _rakServer);

            m_sessions.Add(_guid, _session);

            return ERROR_CODE.SUCEESS;
        }

        public ERROR_CODE GetSession(ulong _guid, out ClientSession _session)
        {
            _session = null;
            if (m_sessions.ContainsKey(_guid) == false) {
                return ERROR_CODE.SESSION_NOT_EXISTS;
            }

            _session = m_sessions[_guid];

            return ERROR_CODE.SUCEESS;
        }

        public ERROR_CODE ClosedConnection(ClientSession _session)
        {
            _session.Close();
            return ERROR_CODE.SUCEESS;
        }

        public ERROR_CODE RemoveSession(ClientSession _session)
        {
            if (m_sessions.Remove(_session.Guid) == false) {
                return ERROR_CODE.UNKNOWN_ERROR;
            }
            return ERROR_CODE.SUCEESS;
        }

        public void Stop()
        {
            foreach (var session in m_sessions) {
                session.Value.Close();
            }

            m_sessions.Clear();
        }
    }
}
