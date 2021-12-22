using GameCommon.Enum;
using LobbyServer.DB.Model;
using System;
using System.Collections.Generic;
using UtilLib;
using UtilLib.Log;

namespace LobbyServer.Lobby
{
    public static class DailyResetShop_Proc
    {
        public const int DummyTotalRate = 2100;
        public const int DummyTotalCount = 8;

        public static bool RefreshShopList(User _user)
        {
            //var suffleShop = _user.DB_Shop.EquipmentShop;
            //if (DateTimeUtil.CompareDateTime(DateTimeUtil.CalcDatetime, suffleShop.NextResetDateTime) == false)
            //{
            //    return false;
            //}

            //var inventory = _user.DB_UserInfo.UniqueItemInventroy;

            //var newItemList = new List<int>();
            //foreach (var itemInfo in dummyShopItemList)
            //{
            //    foreach (var equipmentItem in inventory.UniqueItems)
            //    {
            //        if (equipmentItem.ItemTID == itemInfo.Id)
            //        {
            //            continue;
            //        }

            //        newItemList.Add(itemInfo.Id);
            //    }
            //}

            ////기준치 8개가 만족하지 않는다면 기존에 있는것중에서 랜덤으로 뽑아 넣는다
            //if (newItemList.Count < DummyTotalCount)
            //{
            //    var count = DummyTotalCount - newItemList.Count;
            //    while (count > 0)
            //    {
            //        Random rand = new Random();
            //        var select = rand.Next(0, DummyTotalRate);


            //    }
            //}

            //suffleShop.NextResetDateTime = DateTimeUtil.CalcDatetime;

            return true;
        }

        public static ERROR_CODE CheckBuyItem(FD_EquipmentShop _suffleShop, int _buyItemTID)
        {
            var findTID = _suffleShop.ShopList.Find(x => x.ShopItemTID == _buyItemTID);
            if (findTID == null)
            {
                Log.Instance.Warn($"not found EquipmentShop buyItemTID : {_buyItemTID}");
                return ERROR_CODE.ERROR_CHECK_BUY_SHOP_ITEM;
            }

            return ERROR_CODE.SUCEESS;
        }
    }
}
