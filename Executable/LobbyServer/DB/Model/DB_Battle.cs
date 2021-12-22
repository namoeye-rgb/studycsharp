using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.DB.Model
{
    public class DB_Battle
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public int BattleGrade { get; set; }
        public int WinCount { get; set; }
        public int LoseCount { get; set; }

        public DB_Battle()
        {
            Id = ObjectId.GenerateNewId();
            UserId = ObjectId.GenerateNewId();
        }
    }
}
