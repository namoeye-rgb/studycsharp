using MongoDB.Driver;

namespace DBLib
{
    public class MongoDBBase
    {
        private MongoClient mongoClient;

        public void InitMongoClient(string _connectionString)
        {
            mongoClient = new MongoClient(_connectionString);
        }

        public IMongoDatabase GetDataBase(string _dataBasename)
        {
            if (mongoClient == null) {
                return null;
            }

            return mongoClient.GetDatabase(_dataBasename);
        }

        public void INI()
        {
            //var dd = mongoClient.GetDatabase();
            //var ddf = dd.GetCollection
        }
    }
}
