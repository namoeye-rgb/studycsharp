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
        public static async void PacketReceive(ClientSession _clientSession, PK_CS_LOGIN _pks)
        {
            var res_packet = new PK_SC_LOGIN();

            try
            {
                if (string.IsNullOrEmpty(_pks.AuthKey))
                {
                    res_packet.ErrorCode = ERROR_CODE.INVALID_AUTH_KEY;
                    _clientSession.SendPacket(res_packet);

                    Log.Instance.Debug($"AuthKey is Null");
                    return;
                }

                var result = await DBQuery_Account.DBQuery_SelectAccount(_pks.AuthKey);
                if (result == null)
                {
                    res_packet.ErrorCode = ERROR_CODE.ACCOUNT_NOT_EXISTS;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Debug($"not found DB_Account");
                    return;
                }

                var user = new User(_clientSession);

                //유저 정보 조회
                var dbLoadUserResult = await LoadDBUserData(user, result.Id);
                if (dbLoadUserResult != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = dbLoadUserResult;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Debug($"fail dbUserLoad");
                    return;
                }

                UserMgr.Instance.AddUser(user.UserUID, user);
                _clientSession.SetUserUID(user.UserUID);

                //packet
                res_packet.User = user.Get_Packet_User();
                _clientSession.SendPacket(res_packet);
            }
            catch (Exception e)
            {
                res_packet.ErrorCode = ERROR_CODE.UNKNOWN_ERROR;
                _clientSession.SendPacket(res_packet);
                Log.Instance.Error($"{e}");
            }
        }

        private static async Task<ERROR_CODE> LoadDBUserData(User _user, ObjectId _AccountId)
        {
            var userDB = await DBQuery_Account.DBQuery_SelectUser(_AccountId);
            if (userDB == null)
            {
                Log.Instance.Debug($"FAILED_GET_PLAYER_DATA, AccountId : {_AccountId.ToString() }");
                return ERROR_CODE.FAILED_GET_PLAYER_DATA;
            }

            var mailDB = await DBQuery_Mail.DBQuery_SelectMail(userDB.Id);
            if (mailDB == null)
            {
                Log.Instance.Debug($"FAILED_GET_LOAD_MAIL, UserId : {userDB.Id.ToString() }");
                return ERROR_CODE.FAILED_GET_LOAD_MAIL;
            }

            //var shopDB = await DBQuery_User_Shop.DBQuery_SelectShopInfo(userDB.Id);
            //if (shopDB == null)
            //{
            //    Log.Instance.Debug($"FAILED_GET_SHOP_DATA, AccountId : {userDB.Id.ToString() }");
            //    return ERROR_CODE.FAILED_GET_SHOP_DATA;
            //}

            //_user.SetUserDBData(userDB, shopDB);
            _user.SetUserDBData(userDB, mailDB);

            return ERROR_CODE.SUCEESS;
        }

        private static async Task<ERROR_CODE> InitDBUserData(string _authKey, string _nickName, User _user)
        {
            var accountDB = new DB_Account();
            accountDB.AuthKey = _authKey;

            var dbResult = await DBQuery_Account.DBQuery_InsertAccount(accountDB);
            if (dbResult == false)
            {
                Log.Instance.Debug($" fail DBQuery_InsertAccount");
                return ERROR_CODE.DB_UNKNOWN_ERROR;
            }

            _user.InitUserDBData(accountDB, _nickName);

            dbResult = await DBQuery_User_Object.DBQuery_InsertUser(_user.DB_UserInfo);
            if (dbResult == false)
            {
                Log.Instance.Debug($"fail DBQuery_InsertUser");
                return ERROR_CODE.DB_UNKNOWN_ERROR;
            }

            dbResult = await DBQuery_Mail.DBQuery_InsertMail(_user.DB_Mail);
            if (dbResult == false)
            {
                Log.Instance.Debug($"fail DBQuery_InsertMail");
                return ERROR_CODE.DB_UNKNOWN_ERROR;
            }

            //dbResult = await DBQuery_User_Shop.DBQuery_InsertShop(_user.DB_Shop);
            //if (dbResult == false)
            //{
            //    Log.Instance.Debug($"fail DBQuery_InsertShop");
            //    return ERROR_CODE.DB_UNKNOWN_ERROR;
            //}

            return ERROR_CODE.SUCEESS;
        }

        public static async void PacketReceive(ClientSession _clientSession, PK_CS_CREATE_USER _pks)
        {
            var res_packet = new PK_SC_CREATE_USER();

            try
            {
                if (string.IsNullOrEmpty(_pks.AuthKey) || string.IsNullOrEmpty(_pks.NickName))
                {
                    res_packet.ErrorCode = ERROR_CODE.INVALID_PAKCET_DATA;
                    _clientSession.SendPacket(res_packet);

                    Log.Instance.Debug($"AuthKey is Null");
                    return;
                }

                //중복 닉네임 검사, 임시로
                var users = await DBQuery_User_Object.DBQuery_SelectUserFromNickName(_pks.NickName);
                if (users.Count > 0)
                {
                    res_packet.ErrorCode = ERROR_CODE.ERROR_DUPLICATE_USER_NICKNAME;
                    _clientSession.SendPacket(res_packet);

                    Log.Instance.Debug($"duplicate user nickName");
                    return;
                }

                var user = new User(_clientSession);

                var dbInitUserResult = await InitDBUserData(_pks.AuthKey, _pks.NickName, user);
                if (dbInitUserResult != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = dbInitUserResult;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Debug($"fail InitDBUserData");
                    return;
                }

                UserMgr.Instance.AddUser(user.UserUID, user);
                _clientSession.SetUserUID(user.UserUID);

                res_packet.User = user.Get_Packet_User();
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
