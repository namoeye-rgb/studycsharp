using GameCommon.Enum;
using LobbyServer.DB.Model;
using MongoDB.Bson;
using PacketLib.PacketField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.Lobby
{
    public static class Wallet_Proc
    {
        public static ERROR_CODE CheckPayWallet(FD_Wallet _wallet, PRICE_TYPE _priceType, int _payValue, out Packet_Pay _pay)
        {
            _pay = null;
            var findPriceInfo = _wallet.PriceList.Find(x => x.Type == _priceType);
            if (findPriceInfo == null)
            {
                return ERROR_CODE.ERROR_NOT_FOUND_PRICE_TYPE;
            }

            if (findPriceInfo.Value < _payValue)
            {
                Log.Instance.Error($"NOT_ENOUGH_PRICE_VALUE, my : {findPriceInfo.Value}, pay : {_payValue}");
                return ERROR_CODE.NOT_ENOUGH_PRICE_VALUE;
            }

            _pay = new Packet_Pay();
            _pay.Pay = new Packet_Price
            {
                PriceType = _priceType,
                PriceValue = _payValue
            };

            return ERROR_CODE.SUCEESS;
        }

        public static ERROR_CODE GetReward(ITEM_TYPE _itemType, int _itemTID, int _value, out Packet_Reward _reward)
        {
            _reward = null;
            if (_itemType <= ITEM_TYPE.NONE || _itemType >= ITEM_TYPE.MAX)
            {
                return ERROR_CODE.ERROR_NOT_FOUND_PRICE_TYPE;
            }

            _reward = new Packet_Reward();

            //if (_itemType == ITEM_TYPE.EQUIPMENT_ITEM)
            //{
            //    _reward.UniqueItemList = new List<Packet_UniqueItem>();
            //    for (int i = 0; i < _value; ++i)
            //    {
            //        //TODO : 아이템 등급이나 이런거 가져와야할듯?
            //        var uniqueItem = new Packet_UniqueItem();
            //        uniqueItem.ItemUID = ObjectId.GenerateNewId().ToString();
            //        uniqueItem.ItemTID = _itemTID;
            //        uniqueItem.Grade = 0;
            //        uniqueItem.Level = 0;
            //        uniqueItem.ItemType = _itemType;

            //        _reward.UniqueItemList.Add(uniqueItem);
            //    }
            //}
            //else if (_itemType == ITEM_TYPE.CONSUMER_ITEM 
            //    || _itemType == ITEM_TYPE.PRICE_ITEM)
            //{
            //    _reward.StackItemList = new List<Packet_StackItem>();
            //    var stackItem = new Packet_StackItem
            //    {
            //        ItemTID = _itemTID,
            //        Count = (ushort)_value,
            //        ItemType = _itemType,
            //    };

            //    _reward.StackItemList.Add(stackItem);
            //}

            return ERROR_CODE.SUCEESS;
        }
    }
}
