using EntityService;
using GameCommon;
using GameCommon.Enum;
using LobbyServer.Net;
using NetLib.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilLib;
namespace LobbyServer.Lobby.Shop
{
    public class ShopMgr : Singleton<ShopMgr>
    {
        private Dictionary<SHOP_TYPE, Dictionary<int, int>> shopDatas = new Dictionary<SHOP_TYPE, Dictionary<int, int>>();

        private DateTime lastUpdateDatetime;

        public class DummyData_ShopItem 
        {
            public int ShopTID;
            public int ItemTID;
            public PRICE_TYPE PriceType;
            public int PriceValue;
        }

        public bool Init()
        {
            //TODO : 더미데이터 교체
            shopDatas.Add(SHOP_TYPE.MATERIAL, new Dictionary<int, int> {
                {1,1 },
                { 3,3},
                { 5,5},
                { 7,7},
                { 9,9}
            });

            shopDatas.Add(SHOP_TYPE.GOODS, new Dictionary<int, int> {
                { 2,2 },
                { 4,4 },
                { 6,6 },
                { 8,8},
                { 10,10}
            });

            shopDatas.Add(SHOP_TYPE.DAILY_RESET, new Dictionary<int, int> {
                { 2,2 },
                { 4,4 },
                { 6,6 },
                { 8,8},
                { 10,10}
            });

            return true;
        }

        public List<int> GetShopList(SHOP_TYPE _type)
        {
            if (shopDatas.ContainsKey(_type) == false)
            {
                return null;
            }

            if (_type == SHOP_TYPE.DAILY_RESET)
            {

            }

            return shopDatas[_type].Values.ToList();
        }

        public DummyData_ShopItem GetShopItem(SHOP_TYPE _type, int _shopListTID)
        {
            //TODO : 더미 데이터 없어지면 주석해제
            //if (shopDatas.ContainsKey(_type) == false)
            //{
            //    return null;
            //}

            //var shopData = shopDatas[_type];
            //if (shopData.ContainsKey(_shopItemTID) == false)
            //{
            //    return null;
            //}

            //return shopData[_shopItemTID];

            if (_type == SHOP_TYPE.MATERIAL)
            {
                return new DummyData_ShopItem
                {
                    PriceType = PRICE_TYPE.GOLD,
                    ShopTID = _shopListTID,
                    ItemTID = 1000,
                    PriceValue = 1
                };
            }
            else if (_type == SHOP_TYPE.GOODS)
            {
                return new DummyData_ShopItem
                {
                    PriceType = PRICE_TYPE.GOLD,
                    ShopTID = _shopListTID,
                    ItemTID = 2000,
                    PriceValue = 2
                };
            }

            return null;
        }
    }
}
