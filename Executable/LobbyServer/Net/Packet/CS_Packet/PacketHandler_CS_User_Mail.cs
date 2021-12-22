using GameCommon.Enum;
using LobbyServer.DB.Model;
using LobbyServer.DB.Packet_ExtendMethod;
using LobbyServer.DB.Query;
using LobbyServer.Lobby;
using LobbyServer.Manager;
using LobbyServer.Net;
using MongoDB.Bson;
using PacketLib;
using PacketLib.Message;
using PacketLib.PacketField;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.Packet
{
    public partial class PacketHandler_CS
    {
        public static void PacketReceive(ClientSession _clientSession, PK_CS_MAIL_LIST _pks)
        {
            var res_packet = new PK_SC_MAIL_LIST();

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

                res_packet.MailList = new List<Packet_Mail>();
                foreach (var mail in user.DB_Mail.MailList)
                {
                    var mail_packet = mail.Get_Packet_Mail();
                    res_packet.MailList.Add(mail_packet);
                }

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

        public static async void PacketReceive(ClientSession _clientSession, PK_CS_MAIL_REWARD _pks)
        {
            var res_packet = new PK_SC_MAIL_REWARD();

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

                var result = user.FindMail(_pks.MailUID, out FD_Mail findMail);
                if (result != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = result;
                    Log.Instance.Error($"not found mail, userUID : {_clientSession.UserUID}, mail UID : {_pks.MailUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                user.ApplyMailReward(findMail, out Packet_Reward reward);
                user.ApplyReward(reward);

                await DBQuery_User_Object.DBQuery_UpdateUser(user.DB_UserInfo);
                await DBQuery_Mail.DBQuery_UpdateMail(user.DB_Mail);

                res_packet.Reward = reward;
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

        public static async void PacketReceive(ClientSession _clientSession, PK_CS_MAIL_ALL_REWARD _pks)
        {
            var res_packet = new PK_SC_MAIL_ALL_REWARD();

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

                if (user.DB_Mail.MailList.Count <= 0)
                {
                    res_packet.ErrorCode = ERROR_CODE.NOT_FOUND_MAIL;
                    Log.Instance.Error($"not found mail, userUID : {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                user.ApplyAllMailReward(out Packet_Reward reward);
                user.ApplyReward(reward);

                await DBQuery_User_Object.DBQuery_UpdateUser(user.DB_UserInfo);
                await DBQuery_Mail.DBQuery_UpdateMail(user.DB_Mail);

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
