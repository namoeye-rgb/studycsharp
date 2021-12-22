using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.DB.Model
{
    public class FD_UniqueItem
    {
        public ObjectId Id { get; set; }
        public int ItemTID { get; set; }
        public int Grade { get; set; }
        public int Exp { get; set; }
        public bool IsLock { get; set; }
        public FD_UniqueItem()
        {
            Id = ObjectId.GenerateNewId();
            IsLock = false;
        }
    }

    public class FD_StackItem
    {
        public int ItemTID { get; set; }
        public int Count { get; set; }

        public FD_StackItem()
        {
            Count = 1;
        }
    }


}
