using DBLib;
using LobbyServer.DB.Model;
using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UtilLib;

namespace LobbyServer.DB
{
    public class MongoDB
    {
        //NOTE : 나중에 DBQuqery에서 사용하는 필드에 대해서 인덱스를 걸어야한다

        private MongoDBBase dbBase;

        public const string GameDB = "Game";
        public const string User_Collection = "User";
        public const string Account_Collection = "Account";
        public const string Mail_Collection = "Mail";
        public const string Shop_Collection = "Shop";
        public const string Battle_Collection = "Battle";

        public void Init(string _connectionString)
        {
            dbBase = new MongoDBBase();
            dbBase.InitMongoClient(_connectionString);
        }

        public IMongoCollection<T> GetCollection<T>(string _dataBasename, string _collectionName)
        {
            var db = dbBase.GetDataBase(_dataBasename);
            return db.GetCollection<T>(_collectionName);
        }
    }

    public class MongoDBMgr : Singleton<MongoDBMgr>
    {
        public MongoDB mongoDB;

        public bool Init(string _connectionString, int _port)
        {
            mongoDB = new MongoDB();
            mongoDB.Init(_connectionString + ":" + _port);

            var thread = new Thread(() => {
                while (true) {
                    requestList.TryTake(out Action call);
                    call?.Invoke();
                    Thread.Sleep(1);
                }
            });

            thread.Start();

            return true;
        }

        public IMongoCollection<T> GetCollection<T>(string _dbBaseName, string _collectionName)
        {
            return mongoDB.GetCollection<T>(_dbBaseName, _collectionName);
        }

        private BlockingCollection<Action> requestList = new BlockingCollection<Action>();

        public void DBQuery(Action _dbRequest)
        {
            requestList.Add(_dbRequest);
        }

        public IMongoCollection<DB_User> GetUserCollection()
        {
            return GetCollection<DB_User>(MongoDB.GameDB, MongoDB.User_Collection);
        }

        public IMongoCollection<DB_Shop> GetShopCollection()
        {
            return GetCollection<DB_Shop>(MongoDB.GameDB, MongoDB.Shop_Collection);
        }

        public IMongoCollection<DB_Account> GetAccountCollection()
        {
            return GetCollection<DB_Account>(MongoDB.GameDB, MongoDB.Account_Collection);
        }

        public IMongoCollection<DB_Mail> GetMailCollection()
        {
            return GetCollection<DB_Mail>(MongoDB.GameDB, MongoDB.Mail_Collection);
        }
    }
}
