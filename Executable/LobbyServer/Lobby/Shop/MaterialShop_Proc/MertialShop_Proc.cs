using GameCommon;
using GameCommon.Enum;
using LobbyServer.DB.Model;
using LobbyServer.Lobby.Shop;
using System;
using System.Collections.Generic;
using UtilLib;
using UtilLib.Log;
using static LobbyServer.Lobby.Shop.ShopMgr;

namespace LobbyServer.Lobby
{
    public static class MaterialShop_Proc
    {
        public static ERROR_CODE CheckBuyItem( int _buyItemTID, out DummyData_ShopItem _shopListItem)
        {
            _shopListItem = ShopMgr.Instance.GetShopItem(SHOP_TYPE.MATERIAL, _buyItemTID);
            if (_shopListItem == null)
            {
                Log.Instance.Warn($"not found CheckBuyItem buyItemTID : {_buyItemTID}");
                return ERROR_CODE.ERROR_CHECK_BUY_SHOP_ITEM;
            }

            return ERROR_CODE.SUCEESS;
        }
    }
}
