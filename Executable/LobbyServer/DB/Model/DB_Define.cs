using GameCommon.Enum;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.DB.Model
{
    public class FD_Reward
    {
        public List<FD_Price> PriceList { get; set; }
        public List<FD_RewardItem> ItemList { get; set; }
    }

    public class FD_RewardItem
    {
        public ITEM_TYPE Type{ get; set; }
        public int ItemTID { get; set; }
        public int Count { get; set; }
    }
}