using LobbyServer.DB.Model;
using PacketLib.PacketField;
using System.Collections.Generic;

namespace LobbyServer.Lobby
{
    public partial class User
    {
        public List<Packet_ShopItemInfo> Get_Packet_EquipmentShopItemList()
        {
            //var packet_list = new List<Packet_ShopItemInfo>();
            //foreach (var shopItemInfo in DB_Shop.EquipmentShop.ShopList)
            //{
            //    var packet_item = Get_Packet_ShopItemInfo(shopItemInfo);
            //    packet_list.Add(packet_item);
            //}

            //return packet_list;

            return null;
        }
    }
}
