using GameCommon;
using GameCommon.Enum;
using LobbyServer.DB.Model;
using LobbyServer.DB.Query;
using LobbyServer.Lobby;
using LobbyServer.Lobby.Shop;
using LobbyServer.Manager;
using LobbyServer.Net;
using PacketLib.Message;
using PacketLib.PacketField;
using System;
using UtilLib.Log;

namespace LobbyServer.Packet
{
    public partial class PacketHandler_CS
    {
        public static void PacketReceive(ClientSession _clientSession, PK_CS_SHOP_INFO _pks)
        {
            var res_packet = new PK_SC_SHOP_INFO();

            try
            {
                var shopList = ShopMgr.Instance.GetShopList(_pks.ShopType);
                if (shopList == null)
                {
                    res_packet.ErrorCode = ERROR_CODE.NOT_FOUND_SHOP_LIST;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Warn($"not found shoplist");
                    return;
                }

                res_packet.ShopItemList = shopList;
                _clientSession.SendPacket(res_packet);
            }
            catch (Exception e)
            {
                res_packet.ErrorCode = ERROR_CODE.UNKNOWN_ERROR;
                _clientSession.SendPacket(res_packet);
                Log.Instance.Error($"{e}");
            }
        }

        public static async void PacketReceive(ClientSession _clientSession, PK_CS_SHOP_BUY_ITEM _pks)
        {
            var res_packet = new PK_SC_SHOP_BUY_ITEM();

            try
            {
                var user = UserMgr.Instance.GetUser(_clientSession.UserUID);
                if (user == null)
                {
                    res_packet.ErrorCode = ERROR_CODE.PLAYER_CANNOT_FIND;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Error($"not found user data, userUID : {_clientSession.UserUID}");
                    return;
                }

                ERROR_CODE result = ERROR_CODE.SUCEESS;
                ShopMgr.DummyData_ShopItem shopListItem = null;
                if (_pks.ShopType == SHOP_TYPE.MATERIAL)
                {
                    result = MaterialShop_Proc.CheckBuyItem(_pks.BuyItemTID, out shopListItem);
                }
                else if (_pks.ShopType == SHOP_TYPE.GOODS)
                {
                    result = GoodsShop_Proc.CheckBuyItem(_pks.BuyItemTID, out shopListItem);
                }

                if (result != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = result;
                    Log.Instance.Error($"fail CheckBuyItem: {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                if (shopListItem == null)
                {
                    res_packet.ErrorCode = ERROR_CODE.NOT_FOUND_SHOP_LIST_ITEM;
                    Log.Instance.Error($"not found shoplistItem: {_clientSession.UserUID}");
                    _clientSession.SendPacket(res_packet);
                    return;
                }

                result = Wallet_Proc.CheckPayWallet(user.DB_UserInfo.Wallet, shopListItem.PriceType, shopListItem.PriceValue, out Packet_Pay pay);
                if (result != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = result;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Error($"fail CheckPayWallet: {_clientSession.UserUID}");
                    return;
                }

                result = Wallet_Proc.GetReward(ITEM_TYPE.STACK_ITEM, shopListItem.ItemTID, shopListItem.PriceValue, out Packet_Reward reward);
                if (result != ERROR_CODE.SUCEESS)
                {
                    res_packet.ErrorCode = result;
                    _clientSession.SendPacket(res_packet);
                    Log.Instance.Error($"fail GetReward: {_clientSession.UserUID}");
                    return;
                }

                user.ApplyPayAndReward(pay, reward);

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
