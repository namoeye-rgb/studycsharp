using GameCommon.Enum;
using NetLib.Define;
using PacketLib.Message;
using SyncServer.Manager;
using SyncServer.Net.Session;
using System;
using UtilLib.Log;

namespace SyncServer.Packet
{
    public partial class PacketHandler_S2S
    {
        public static void PacketReceive(ClientSession _clientSession, PK_S2S_REQ_CONNECT _pks)
        {
            var s2s_res_connect = new PK_S2S_RES_CONNECT();

            try {
                SessionMgr.Instance.AddSession(_clientSession, (SERVER_TYPE)_pks.ServerType);
                s2s_res_connect.Id = _clientSession.SessionUID;
                _clientSession.SendPacket(s2s_res_connect);

                var serverType = (NetLib.Define.SERVER_TYPE)_pks.ServerType;

                Log.Instance.Debug($"PK_S2S_RES_CONNECT Connect, Type : {serverType.ToString()}, Id : {_clientSession.SessionUID}");
            } catch (Exception e) {
                s2s_res_connect.ErrorCode = ERROR_CODE.UNKNOWN_ERROR;
                _clientSession.SendPacket(s2s_res_connect);
                Log.Instance.Error($"{e}");
            }
        }
    }
}
