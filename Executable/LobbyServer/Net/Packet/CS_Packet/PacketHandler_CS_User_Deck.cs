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
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilLib.Log;


namespace LobbyServer.Packet
{
    public partial class PacketHandler_CS
    {
        public static void PacketReceive(ClientSession _clientSession, PK_CS_USER_DECK_INFO _pks)
        {
            var res_packet = new PK_SC_USER_DECK_INFO();

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

                res_packet.DeckList = user.DB_UserInfo.DeckInfo.DeckList;
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

        public static async void PacketReceive(ClientSession _clientSession, PK_CS_USER_DECK_SAVE _pks)
        {
            var res_packet = new PK_SC_USER_DECK_SAVE();

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

                //0~ 19인덱스(20개)가 하나의 그룹, 총 4개의 그룹이 있다
                if (_pks.DeckGroupId >= 4 || _pks.DeckGroupId < 0)
                {
                    res_packet.ErrorCode = ERROR_CODE.NOT_FOUND_SAVE_DECK_GROUP;
                    Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                if (_pks.DeckList.Count != 20)
                {
                    res_packet.ErrorCode = ERROR_CODE.INVALID_SAVE_DECK_LIST_COUT;
                    Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                var deckInfo = user.DB_UserInfo.DeckInfo;

                //혹시나 프로젝트 계속하면 검증 넣자
                foreach (var deckuid in _pks.DeckList)
                {
                    if (ObjectId.TryParse(deckuid, out ObjectId id) == false)
                    {
                        if (id == ObjectId.Empty)
                        {
                            continue;
                        }

                        res_packet.ErrorCode = ERROR_CODE.INVALID_SAVE_DECK_LIST_UID;
                        Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                        _clientSession.SendPacket(res_packet);
                        return;
                    }
                    var uniqueItem = user.DB_UserInfo.UniqueItemInventroy.UniqueItems.Find(x => x.Id == id);

                    if (uniqueItem == null)
                    {
                        res_packet.ErrorCode = ERROR_CODE.INVALID_SAVE_DECK_LIST_UID;
                        Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                        _clientSession.SendPacket(res_packet);
                        return;
                    }
                }

                for (int i = 20 * _pks.DeckGroupId, j = 0; j < _pks.DeckList.Count; ++i, ++j)
                {
                    deckInfo.DeckList[i] = _pks.DeckList[j];
                }

                await DBQuery_User_Object.DBQuery_UpdateUser(user.DB_UserInfo);

                res_packet.ChangeDeckList = new List<string>(_pks.DeckList);

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
    }
}
