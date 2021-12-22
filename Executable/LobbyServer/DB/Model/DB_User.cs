using GameCommon.Enum;
using MongoDB.Bson;
using System.Collections.Generic;

namespace LobbyServer.DB.Model
{
    public class DB_Account
    {
        public ObjectId Id { get; set; }
        public string AuthKey { get; set; }
        public DB_Account()
        {
            Id = ObjectId.GenerateNewId();
            AuthKey = string.Empty;
        }
    }

    public class DB_User
    {
        public ObjectId Id { get; set; }
        public ObjectId AccountId { get; set; }
        public string NickName { get; set; }
        public int Level { get; set; }
        public ulong Exp { get; set; }
        public FD_Character Characters { get; set; }
        public FD_Wallet Wallet { get; set; }
        public FD_StackItem_Inventory StackItemInventroy { get; set; }
        public FD_UniqueItem_Inventory UniqueItemInventroy { get; set; }
        public FD_BattleInfo BattleInfo { get; set; }
        public FD_DeckInfo DeckInfo { get; set; }

        public DB_User ()
        {
            Id = ObjectId.GenerateNewId();
            AccountId = ObjectId.Empty;
            NickName = string.Empty;
            Level = 1;
            Exp = 0;
            Characters = new FD_Character();
            BattleInfo = new FD_BattleInfo();
            Wallet = new FD_Wallet();
            StackItemInventroy = new FD_StackItem_Inventory();
            UniqueItemInventroy = new FD_UniqueItem_Inventory();
            DeckInfo = new FD_DeckInfo();
        }
    }

    public class FD_Character
    {
        public ObjectId Id { get; set; }
        public int Level { get; set; }
        public ulong Exp { get; set; }

        public FD_Character()
        {
            Id = ObjectId.GenerateNewId();
            Level = 1;
        }
    }

    public class FD_DeckInfo
    {
        public List<string> DeckList { get; set; }

        public FD_DeckInfo()
        {
            DeckList = new List<string>();
        }
    }

    public class FD_BattleInfo
    {
        public int BattleGrade { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }
    }

    public class FD_Wallet
    {
        public List<FD_Price> PriceList { get; set; }

        public FD_Wallet()
        {
            PriceList = new List<FD_Price>();
        }
    }

    public class FD_Price
    {
        public PRICE_TYPE Type { get; set; }
        public int Value { get; set; }
    }

    public class FD_StackItem_Inventory
    {
        public List<FD_StackItem> StackItems { get; set; }

        public int Level { get; set; }

        public FD_StackItem_Inventory()
        {
            StackItems = new List<FD_StackItem>();
            Level = 1;
        }
    }
    public class FD_UniqueItem_Inventory
    {
        public List<FD_UniqueItem> UniqueItems { get; set; }

        public int Level { get; set; }

        public FD_UniqueItem_Inventory()
        {
            UniqueItems = new List<FD_UniqueItem>();
            Level = 1;
        }
    }
}
