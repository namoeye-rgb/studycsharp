using GameCommon.Enum;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LobbyServer.DB.Model
{
    public class DB_Mail
    {
        public ObjectId Id { get; set; }
        public ObjectId UserID { get; set; }
        public List<FD_Mail> MailList { get; set; }

        public DB_Mail()
        {
            MailList = new List<FD_Mail>();
            Id = ObjectId.GenerateNewId();
        }
    }

    public class FD_Mail
    {
        public ObjectId Id { get; set; }
        public int MailTID { get; set; }
        public MAIL_TYPE Type { get; set; }
        public DateTime RemainTime { get; set; }
        public FD_MailCustomInfo CustomInfo { get; set; }
        public FD_Reward Reward { get; set; }

        public FD_Mail()
        {
            Id = ObjectId.GenerateNewId();
        }
    }

    public class FD_MailCustomInfo
    {
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
