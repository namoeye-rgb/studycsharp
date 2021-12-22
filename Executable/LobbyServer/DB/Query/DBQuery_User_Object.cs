using LobbyServer.DB.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.DB.Query
{
    public class DBQuery_User_Object
    {
        public static async Task<bool> DBQuery_InsertUser(DB_User _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetUserCollection();
                await collection.InsertOneAsync(_model);
                Log.Instance.Debug("DBQuery_UserUpdate Success");

                return true;
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_UserUpdate error, {e}");
                return false;
            }
        }

        public static async Task<bool> DBQuery_UpdateUser(DB_User _model)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetUserCollection();

                var filter = Builders<DB_User>.Filter.Where(x => x.Id == _model.Id);
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

        public static async Task<List<DB_User>> DBQuery_SelectUserFromNickName(string _nickName)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetUserCollection();

                var filter = Builders<DB_User>.Filter.Where(x => x.NickName == _nickName);
                var finduser = await collection.FindAsync(filter);
                Log.Instance.Debug("DBQuery_UpdateUser Success");
                return finduser.ToList();
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_UpdateUser error, {e}");
                return null;
            }
        }
    }
}
