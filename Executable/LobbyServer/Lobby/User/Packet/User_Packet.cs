using LobbyServer.DB.Model;
using LobbyServer.DB.Packet_ExtendMethod;
using PacketLib.PacketField;
using System.Collections.Generic;

namespace LobbyServer.Lobby
{
    public partial class User
    {
        public Packet_User Get_Packet_User()
        {
            var packet = new Packet_User();
            packet.UID = DB_UserInfo.Id.ToString();
            packet.NickName = DB_UserInfo.NickName;
            packet.Level = DB_UserInfo.Level;
            packet.Exp = DB_UserInfo.Exp;

            packet.Character = DB_UserInfo.Characters.Get_Packet_Character();
            packet.Wallet = Get_Packet_Wallet();

            packet.Inventory = Get_Packet_Inventory();
            packet.DeckInfo = new Packet_DeckInfo();
            packet.DeckInfo.DeckList= DB_UserInfo.DeckInfo.DeckList;

            return packet;
        }

        public Packet_Wallet Get_Packet_Wallet()
        {
            var wallet = new Packet_Wallet();
            wallet.PriceList = new List<Packet_Price>();

            foreach (var price in DB_UserInfo.Wallet.PriceList)
            {
                wallet.PriceList.Add(new Packet_Price { 
                    PriceType = price.Type,
                    PriceValue = price.Value
                });
            }

            return wallet;
        }

        public Packet_BattleInfo Get_Packet_Battle()
        {
            var battle = new Packet_BattleInfo();
            battle.Grade = DB_UserInfo.BattleInfo.BattleGrade;
            battle.WinCount = DB_UserInfo.BattleInfo.WinCount;
            battle.LoseCount = DB_UserInfo.BattleInfo.LoseCount;

            return battle;
        }

        public Packet_Inventory Get_Packet_Inventory()
        {
            var packet_inventory = new Packet_Inventory
            {
                StackItemInventory = Get_Packet_StackItemInventory(),
                UniqueItemInventory = Get_Packet_UniqueItemInventory()
            };

            return packet_inventory;
        }

        public Packet_UniqueItemInventory Get_Packet_UniqueItemInventory()
        {
            var packet_UniqueItemInventory = new Packet_UniqueItemInventory
            {
                UniqueItemList = new List<Packet_UniqueItem>()
            };

            foreach (var itemInfo in DB_UserInfo.UniqueItemInventroy.UniqueItems)
            {
                var packet_item = Get_Packet_UniqueItem(itemInfo);
                packet_UniqueItemInventory.UniqueItemList.Add(packet_item);
            }

            return packet_UniqueItemInventory;
        }


        public Packet_UniqueItem Get_Packet_UniqueItem(FD_UniqueItem _item)
        {
            return new Packet_UniqueItem
            {
                ItemUID = _item.Id.ToString(),
                ItemTID = _item.ItemTID,
                Exp = (ushort)_item.Exp,
                Grade = (byte)_item.Grade,
                IsLock = _item.IsLock,
            };
        }

        public Packet_StackItemInventory Get_Packet_StackItemInventory()
        {
            var packet_StackItemInventory = new Packet_StackItemInventory
            {
                StackItemList = new List<Packet_StackItem>()
            };

            foreach (var itemInfo in DB_UserInfo.StackItemInventroy.StackItems)
            {
                var packet_item = Get_Packet_StackItem(itemInfo);
                packet_StackItemInventory.StackItemList.Add(packet_item);
            }

            return packet_StackItemInventory;
        }


        public Packet_StackItem Get_Packet_StackItem(FD_StackItem _item)
        {
            return new Packet_StackItem
            {
                Count = (ushort)_item.Count,
                ItemTID = _item.ItemTID,
            };
        }
    }
}
