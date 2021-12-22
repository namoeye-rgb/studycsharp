using GameServer.Net;
using PacketLib.Message;
using System;
using UtilLib.Log;

namespace GameServer.Packet
{
    public partial class PacketHandler_S2S
    {
        public static void PacketReceive(PK_S2S_RES_CONNECT _pks)
        {
            S2SClient.Instance.S2SClientNumber = (ushort)_pks.Id;

            Log.Instance.Debug($"PacketReceive GameServer PK_S2S_RES_CONNECT, {_pks.Id}");
        }


        //NOTE : RES, REQ가 혼재되어 약간 혼란스러울수 있는데 고민좀해봐야겠음.
        public static void PacketReceive(PK_S2G_MAKE_BATTLE_ROOM _pks)
        {
            try {
                Log.Instance.Info($"REQ_MAKE_BATTLE_ROOM, GUID : {_pks.ReqBattleRoomGUID}, ROOMTYPE : {_pks.RoomType}");
                //NOTE : 실제 방 생성 후에 패킷을 보내야한다
                var packet = new PK_G2S_MAKE_BATTLE_ROOM();
                packet.MakeBattleRoomGUID = _pks.ReqBattleRoomGUID;
                packet.MakeRoomType = _pks.RoomType;

                S2SClient.Instance.SendPacket(packet);
            } catch (Exception e) {
                Log.Instance.Error($"{e}");
            }
        }
    }
}
