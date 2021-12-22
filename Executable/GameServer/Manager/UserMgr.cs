using EntityService;
using GameCommon.Enum;
using GameServer.Game;
using PacketLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLib;

namespace GameServer.Manager
{
    public class UserMgr : Singleton<UserMgr>
    {
        private readonly Dictionary<ESHandle, User> users = new Dictionary<ESHandle, User>();

        public ERROR_CODE RemoveUser(ulong _userUID)
        {
            if (users.Remove(_userUID) == false)
            {
                return ERROR_CODE.UNKNOWN_ERROR;
            }
            return ERROR_CODE.SUCEESS;
        }
    }
}
