using LobbyServer.DB.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.DB.Query
{
    public class DBQuery_Battle
    {
        public static async Task<DB_Battle> DBQuery_SelectBattle(ObjectId _id)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetBattleCollection();
                var result = await collection.FindAsync(x => x.UserId == _id);

                Log.Instance.Debug("DBQuery_SelectBattle Success");

                return result.FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_SelectBattle error, {e}");
                return null;
            }
        }

        public static async Task<bool> DBQuery_InsertBattle(DB_Battle _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetBattleCollection();
                await collection.InsertOneAsync(_model);
                Log.Instance.Debug("DBQuery_InsertBattle Success");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_InsertBattle error, {e}");
                return false;
            }
        }
    }
}
