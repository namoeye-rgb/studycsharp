using LobbyServer.DB.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.DB.Query
{
    public class DBQuery_Mail
    {
        public static async Task<DB_Mail> DBQuery_SelectMail(ObjectId _id)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetMailCollection();
                var result = await collection.FindAsync(x => x.UserID == _id);

                Log.Instance.Debug("DBQuery_SelectMail Success");

                return result.FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_SelectMail error, {e}");
                return null;
            }
        }

        public static async Task<bool> DBQuery_UpdateMail(DB_Mail _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetMailCollection();

                var filter = Builders<DB_Mail>.Filter.Where(x => x.Id == _model.Id);
                await collection.ReplaceOneAsync(filter, _model);

                Log.Instance.Debug("DBQuery_UpdateMail Success");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_UpdateMail error, {e}");
                return false;
            }
        }

        public static async Task<bool> DBQuery_InsertMail(DB_Mail _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetMailCollection();
                await collection.InsertOneAsync(_model);
                Log.Instance.Debug("DBQuery_InsertMail Success");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_InsertMail error, {e}");
                return false;
            }
        }
    }
}
