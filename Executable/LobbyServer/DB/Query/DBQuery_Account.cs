using LobbyServer.DB.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using UtilLib.Log;

namespace LobbyServer.DB.Query
{
    public class DBQuery_Account
    {
        public static async Task<bool> DBQuery_InsertAccount(DB_Account _model)
        {
            try {
                var collection = MongoDBMgr.Instance.GetAccountCollection();
                await collection.InsertOneAsync(_model);

                Log.Instance.Debug("DBQuery_InsertAccount Success");

                return true;
            } catch (Exception e) {
                Log.Instance.Warn($"DBQuery_InsertAccount error, {e}");
                return false;
            }
        }

        //NOTE : 우선 로그인 Api전까지는 닉네임으로 찾는다
        public static async Task<DB_Account> DBQuery_SelectAccount(string _authKey)
        {
            try {
                var collection = MongoDBMgr.Instance.GetAccountCollection();
                var result = await collection.FindAsync(x => x.AuthKey == _authKey);

                Log.Instance.Debug("DBQuery_SelectAccount Success");

                return result.FirstOrDefault();
            } catch (Exception e) {
                Log.Instance.Warn($"DBQuery_SelectAccount error, {e}");
                return null;
            }
        }

        //NOTE : 우선 로그인 Api전까지는 닉네임으로 찾는다
        public static async Task<DB_User> DBQuery_SelectUser(ObjectId _id)
        {
            try
            {
                var collection = MongoDBMgr.Instance.GetUserCollection();
                var result = await collection.FindAsync(x => x.AccountId == _id);

                Log.Instance.Debug("DBQuery_UserSelect Success");

                return result.FirstOrDefault();
            }
            catch (Exception e)
            {
                Log.Instance.Warn($"DBQuery_UserSelect error, {e}");
                return null;
            }
        }
    }
}
