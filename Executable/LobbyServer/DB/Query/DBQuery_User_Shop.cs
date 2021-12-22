using LobbyServer.DB.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.DB.Query
{
    public class DBQuery_User_Shop
    {
        public static async Task<bool> DBQuery_InsertShop(DB_Shop _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetShopCollection();
                await collection.InsertOneAsync(_model);
                Log.Instance.Debug("DBQuery_InsertShop Success");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_InsertShop error, {e}");
                return false;
            }
        }

        public static async Task<DB_Shop> DBQuery_SelectShopInfo(ObjectId _id)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetShopCollection();
                var result = await collection.FindAsync(x => x.UserUID == _id);

                Log.Instance.Debug("DBQuery_SelectShopInfo Success");

                return result.FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_SelectShopInfo error, {e}");
                return null;
            }
        }

        public static async Task<bool> DBQuery_UpdateShop(DB_Shop _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetShopCollection();

                var filter = Builders<DB_Shop>.Filter.Where(x => x.Id == _model.Id);
                await collection.ReplaceOneAsync(filter, _model);

                Log.Instance.Debug("DBQuery_UpdateUser Success");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_UpdateUser error, {e}");
                return false;
            }
        }
    }
}