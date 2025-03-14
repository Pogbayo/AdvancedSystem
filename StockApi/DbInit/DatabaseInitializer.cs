using MongoDB.Driver;
using StockApi.Interfaces;

namespace StockApi.DbInit
{
    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IMongoDatabase _database;
        public DatabaseInitializer(IMongoClient mongoClient)
        {
            _database = mongoClient.GetDatabase("AuthDb");
        }
        public async Task Initialize()
        {
            var existingCollections = await _database.ListCollectionNames().ToListAsync();
            string[] requiredCollections = { "CartItems", "Categories", "Orders", "OrderItems", "Products", "Users" };

            foreach (var collection in requiredCollections)
            {
                if (!existingCollections.Contains(collection))
                {
                    await _database.CreateCollectionAsync(collection);
                }
            }

        }
    }
}
