using GameCommon.Enum;
using LobbyServer.DB.Model;
using LobbyServer.DB.Query;
using LobbyServer.Lobby;
using LobbyServer.Manager;
using LobbyServer.Net;
using MongoDB.Bson;
using PacketLib;
using PacketLib.Message;
using System;
using System.Threading.Tasks;
using UtilLib.Log;


namespace LobbyServer.Packet
{
    public partial class PacketHandler_CS
    {
        public static void PacketReceive(ClientSession _clientSession, PK_CS_CHARACTER_INVEN_LIST _pks)
        {
            var res_packet = new PK_SC_CHARACTER_INVEN_LIST();

            try
            {
                var user = UserMgr.Instance.GetUser(_clientSession.UserUID);
                if (user == null)
                {
                    res_packet.ErrorCode = ERROR_CODE.PLAYER_CANNOT_FIND;
                    Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                res_packet.UniqueItemInventory = user.Get_Packet_UniqueItemInventory();
                res_packet.ErrorCode = ERROR_CODE.SUCEESS;

                _clientSession.SendPacket(res_packet);

            }
            catch (Exception e)
            {
                res_packet.ErrorCode = ERROR_CODE.UNKNOWN_ERROR;
                _clientSession.SendPacket(res_packet);
                Log.Instance.Error($"{e}");
            }
        }

        public static async void PacketReceive(ClientSession _clientSession, PK_CS_CHARACTER_DELETE_INVEN_ITEM _pks)
        {
            var res_packet = new PK_SC_CHARACTER_DELETE_INVEN_ITEM();

            try
            {
                var user = UserMgr.Instance.GetUser(_clientSession.UserUID);
                if (user == null)
                {
                    res_packet.ErrorCode = ERROR_CODE.PLAYER_CANNOT_FIND;
                    Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                var result = user.DeleteUniqueItem(_pks.ItemUID, out FD_UniqueItem deleteItem);
                if (result != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = result;
                    Log.Instance.Error($"fail deleteUniqueItem, userUID : {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                }

                res_packet.ErrorCode = ERROR_CODE.SUCEESS;
                res_packet.DeleteUniqueItem = user.Get_Packet_UniqueItem(deleteItem);

                await DBQuery_User_Object.DBQuery_UpdateUser(user.DB_UserInfo);
                _clientSession.SendPacket(res_packet);

            }
            catch (Exception e)
            {
                res_packet.ErrorCode = ERROR_CODE.UNKNOWN_ERROR;
                _clientSession.SendPacket(res_packet);
                Log.Instance.Error($"{e}");
            }
        }
    }
}
