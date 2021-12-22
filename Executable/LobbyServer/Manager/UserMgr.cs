using LobbyServer.Lobby;
using LobbyServer.Net;
using PacketLib.Message;
using System.Collections.Generic;
using UtilLib;
using UtilLib.Log;

namespace LobbyServer.Manager
{
    public class UserMgr : Singleton<UserMgr>
    {
        private Dictionary<string, User> users = new Dictionary<string, User>();
        private object userLock = new object();

        public int UserCount {
            get {
                return users.Count;
            }
        }

        public void AddUser(string _userUID, User _user)
        {
            if (users.ContainsKey(_userUID)) {
                Log.Instance.Debug($"duplicate user info, {_userUID}");
                return;
            }

            users.Add(_userUID, _user);
        }

        public void OnDisconnect(string _userUID)
        {
            if (string.IsNullOrEmpty(_userUID)) {
                return;
            }

            if (users.ContainsKey(_userUID) == false) {
                return;
            }

            var user = users[_userUID];
            user = null;
            users.Remove(_userUID);
        }

        public User GetUser(string _userUID)
        {
            if (users.ContainsKey(_userUID) == false) {
                return null;
            }

            return users[_userUID];
        }
    }
}
