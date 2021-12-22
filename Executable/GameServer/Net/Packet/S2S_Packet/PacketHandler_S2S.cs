using GameServer.Manager;
using GameServer.Net;
using NetLib.Token;
using PacketLib.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLib.Log;

namespace GameServer.Packet
{
    public partial class PacketHandler_S2S
    {
        public static void OnPacketReceive(IUserToken _userToken, PK_S2S_RES_CONNECT _pks)
        {
            S2SClient.Instance.S2SClientNumber = (ushort)_pks.Id;

            Log.Instance.Debug($"OnPacketReceive GameServer PK_S2S_RES_CONNECT, {_pks.Id}");
        }


        //NOTE : RES, REQ가 혼재되어 약간 혼란스러울수 있는데 고민좀해봐야겠음.
        public static void OnPacketReceive(IUserToken _userToken, PK_S2S_REQ_MAKE_BATTLE_ROOM _pks)
        {
            Log.Instance.Debug($"OnPacketReceive GameServer PK_S2S_REQ_MAKE_BATTLE_ROOM, ReqMatchRoomID : {_pks.ReqRoomId}");
        }
    }
}
