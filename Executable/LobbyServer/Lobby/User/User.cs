using LobbyServer.DB.Model;
using LobbyServer.Net;
using System;
using System.Linq;

namespace LobbyServer.Lobby
{
    public partial class User
    {
        public DB_User DB_UserInfo { get; private set; }
        public DB_Mail DB_Mail { get; private set; }

        public ClientSession Session { get; private set; }
        public string UserUID { get; private set; }

        public User(ClientSession _session, bool isNewUser = false)
        {
            Session = _session;
        }

        public void SetUserDBData(
            DB_User _userDB,
            DB_Mail _mailDB)
        {
            DB_UserInfo = _userDB;
            UserUID = DB_UserInfo.Id.ToString();

            DB_Mail = _mailDB;
        }

        public void InitUserDBData(DB_Account _accountDB, string _nickName)
        {
            DB_UserInfo = new DB_User
            {
                AccountId = _accountDB.Id,
                NickName = _nickName,
            };

            UserUID = DB_UserInfo.Id.ToString();

            DB_Mail = new DB_Mail
            {
                UserID = DB_UserInfo.Id
            };

            SetDefaultUser();
        }

        private void SetDefaultUser()
        {
            //TODO : 데이터를 통해서 기본 유저값을 넣어야할듯함

            //SuffleShop_Proc.RefreshShopList(DB_Shop.SuffleShop);
            //Wallet
            DB_UserInfo.Wallet.PriceList.Add(new FD_Price { 
                Type = GameCommon.Enum.PRICE_TYPE.GOLD,
                Value = 1000,
            });

            DB_UserInfo.UniqueItemInventroy.Level = 2;

            var characterData = GameDataLib.GameDataLoader.Instance.Champdata;

            foreach(var character in characterData)
            {
                var uniqueItem = new FD_UniqueItem();
                uniqueItem.ItemTID = character.idx;
                uniqueItem.Grade = character.grade;
                uniqueItem.IsLock = false;
                uniqueItem.Exp = 0;

                DB_UserInfo.UniqueItemInventroy.UniqueItems.Add(uniqueItem);
            }

            for(int i = 0; i < 80; ++i)
            {
                DB_UserInfo.DeckInfo.DeckList.Add(string.Empty);
            }

            var testMail = new FD_Mail();
            testMail.MailTID = 123;

            testMail.Reward = new FD_Reward();
            testMail.Reward.ItemList = new System.Collections.Generic.List<FD_RewardItem>();
            testMail.Reward.PriceList = new System.Collections.Generic.List<FD_Price>();

            testMail.RemainTime = DateTime.Now.AddDays(2);
            testMail.Reward.PriceList.Add(new FD_Price { 
                Type = GameCommon.Enum.PRICE_TYPE.GOLD,
                Value = 100,
            });

            testMail.Reward.ItemList.Add(new FD_RewardItem
            {
                ItemTID = characterData.FirstOrDefault().idx,
                Type = GameCommon.Enum.ITEM_TYPE.CHARACTER_ITEM,
                Count = 1,
            });

            testMail.Reward.ItemList.Add(new FD_RewardItem
            {
                ItemTID = 333,
                Type = GameCommon.Enum.ITEM_TYPE.STACK_ITEM,
                Count = 10,
            });

            DB_Mail.MailList.Add(testMail);
        }
    }
}
