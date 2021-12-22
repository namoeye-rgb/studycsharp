using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace LobbyServer.DB.Model
{
    public class DB_Shop
    {
        public ObjectId Id { get; set; }
        
        public ObjectId UserUID { get; set; }

        public FD_EquipmentShop EquipmentShop { get; set; }

        public DB_Shop()
        {
            Id = ObjectId.GenerateNewId();
            UserUID = ObjectId.Empty;
            EquipmentShop = new FD_EquipmentShop();
        }
    }

    public class FD_EquipmentShop
    {
        public List<FD_EquipmentShopItemInfo> ShopList;
        public DateTime NextResetDateTime;

        public FD_EquipmentShop()
        {
            ShopList = new List<FD_EquipmentShopItemInfo>();
        }
    }

    public class FD_EquipmentShopItemInfo
    {
        public int ShopItemTID { get; set; }
        public int RemainBuyCount { get; set; }
    }
}
