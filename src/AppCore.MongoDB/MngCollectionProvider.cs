using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver;

namespace AppCore.MongoDB
{
    public class MngCollectionProvider : IMngCollectionProvider
    {
        private readonly IMongoDatabase _database;

        private readonly Dictionary<Type, object> _collections;
        public IMongoDatabase Database { get { return _database; } }
        public Dictionary<Type, object> Collections { get { return _collections; } }

        public MngCollectionProvider(IMongoDatabase database)
        {
            _database = database;
            _collections = new Dictionary<Type, object>();
        }

        public virtual void RegisterCollection<TDocument>(string collectionName, MongoCollectionSettings collectionSettings = null)
        {
            Type collectionType = typeof(TDocument);

            if (!Collections.ContainsKey(collectionType))
            {
                Collections.Add(collectionType, _database.GetCollection<TDocument>(collectionName, collectionSettings));
            }
        }

        public virtual IMongoCollection<TDocument> GetCollection<TDocument>()
        {
            return (IMongoCollection<TDocument>)Collections[typeof(TDocument)];
        }
    }
}
