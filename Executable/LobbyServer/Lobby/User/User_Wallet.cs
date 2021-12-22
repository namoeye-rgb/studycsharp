using GameCommon.Enum;
using LobbyServer.DB.Model;
using PacketLib.PacketField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.Lobby
{
    public partial class User
    {
        public ERROR_CODE ApplyPayAndReward(Packet_Pay _pay, Packet_Reward _reward)
        {
            if (_pay == null || _reward == null)
            {
                return ERROR_CODE.SERVER_INTERNAL_ERROR;
            }

            ApplyPay(_pay);
            ApplyReward(_reward);

            return ERROR_CODE.SUCEESS;
        }

        public void ApplyPay(Packet_Pay _pay)
        {
            var priceInfo = DB_UserInfo.Wallet.PriceList.Find(x => x.Type == _pay.Pay.PriceType);
            priceInfo.Value -= _pay.Pay.PriceValue;
        }

        public void ApplyReward(Packet_Reward _reward)
        {
            if (_reward.PriceList != null)
            {
                foreach (var price in _reward.PriceList)
                {
                    AddWallet(price.PriceType, price.PriceValue);
                }
            }

            if (_reward.StackItemList != null)
            {
                foreach (var stackItem in _reward.StackItemList)
                {
                    AddStackItem(stackItem.ItemTID, stackItem.Count);
                }
            }

            if (_reward.UniqueItemList != null)
            {
                foreach (var uniqueItem in _reward.UniqueItemList)
                {
                    AddUniqueItem(uniqueItem.ItemUID, uniqueItem.ItemTID, uniqueItem.Grade, uniqueItem.Exp);
                }
            }
        }

        private void AddWallet(PRICE_TYPE _priceType, int _value)
        {
            var findWallet = DB_UserInfo.Wallet.PriceList.Find(x => x.Type == _priceType);

            if (findWallet == null)
            {
                var newPriceType = new FD_Price
                {
                    Type = _priceType,
                    Value = _value
                };

                DB_UserInfo.Wallet.PriceList.Add(newPriceType);
                return;
            }

            findWallet.Value += _value;
        }
    }
}
