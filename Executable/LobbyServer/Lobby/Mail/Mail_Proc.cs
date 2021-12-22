using GameCommon.Enum;
using LobbyServer.DB.Model;
using MongoDB.Bson;
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
        public ERROR_CODE FindMail(string _mailUID, out FD_Mail _findMail)
        {
            _findMail = null;
            if (ObjectId.TryParse(_mailUID, out ObjectId id) == false)
            {
                return ERROR_CODE.INVALID_MAIL_UID;
            }

            _findMail = DB_Mail.MailList.Find(x => x.Id == id);

            if (_findMail == null)
            {
                return ERROR_CODE.NOT_FOUND_MAIL;
            }

            return ERROR_CODE.SUCEESS;
        }

        public void ApplyMailReward(FD_Mail _mail, out Packet_Reward _reward)
        {
            _reward = new Packet_Reward();
            Mail_Proc.ApplyMailReward(_mail, _reward);

            DB_Mail.MailList.Remove(_mail);
        }

        public ERROR_CODE ApplyAllMailReward(out Packet_Reward _reward)
        {
            _reward = null;
            if (DB_Mail.MailList.Count <= 0)
            {
                return ERROR_CODE.NOT_FOUND_MAIL;
            }

            _reward = new Packet_Reward();
            foreach (var mail in DB_Mail.MailList)
            {
                Mail_Proc.ApplyMailReward(mail, _reward);
            }

            DB_Mail.MailList.Clear();

            return ERROR_CODE.SUCEESS;
        }
    }

    public static class Mail_Proc
    {
        public static void ApplyMailReward(FD_Mail _mail, Packet_Reward _reward)
        {
            foreach (var price in _mail.Reward.PriceList)
            {
                if (_reward.PriceList == null)
                {
                    _reward.PriceList = new List<Packet_Price>();
                }

                var findPrice = _reward.PriceList.Find(x => x.PriceType == price.Type);
                if (findPrice != null)
                {
                    findPrice.PriceValue += price.Value;
                    continue;
                }

                _reward.PriceList.Add(new Packet_Price
                {
                    PriceType = price.Type,
                    PriceValue = price.Value,
                });
            }

            foreach (var item in _mail.Reward.ItemList)
            {
                if (item.Type == ITEM_TYPE.CHARACTER_ITEM)
                {
                    if (_reward.UniqueItemList == null)
                    {
                        _reward.UniqueItemList = new List<Packet_UniqueItem>();
                    }

                    for (int i = 0; i < item.Count; ++i)
                    {
                        _reward.UniqueItemList.Add(new Packet_UniqueItem
                        {
                            ItemUID = ObjectId.GenerateNewId().ToString(),
                            ItemType = item.Type,
                            ItemTID = item.ItemTID,
                        });
                    }
                }
                else if (item.Type == ITEM_TYPE.STACK_ITEM)
                {
                    if (_reward.StackItemList == null)
                    {
                        _reward.StackItemList = new List<Packet_StackItem>();
                    }

                    var findItem = _reward.StackItemList.Find(x => x.ItemTID == item.ItemTID);
                    if (findItem != null)
                    {
                        findItem.Count += (ushort)item.Count;
                        continue;

                    }
                    _reward.StackItemList.Add(new Packet_StackItem
                    {
                        Count = (ushort)item.Count,
                        ItemTID = item.ItemTID,
                        ItemType = item.Type,
                    });
                }
            }
        }
    }
}
