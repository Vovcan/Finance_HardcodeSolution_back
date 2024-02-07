using Finance_back.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Finance_back.Services

{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _userCollection;
        //conect to database
        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>(mongoDBSettings.Value.CollectionName);
        }
        //create new user
        public async Task CreateAsync(User user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            await _userCollection.InsertOneAsync(user);
            return;
        }
        //get for all users
        public async Task<List<User>> GetAsync()
        {
            return await _userCollection.Find(new BsonDocument()).ToListAsync();
        }
        //get 1 user by id
        public async Task<User> FindUserById(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            User user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
        public async Task<User> UpdateUser(User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, updatedUser.Id);
            var result = await _userCollection.ReplaceOneAsync(filter, updatedUser);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return updatedUser;
            }
            return null;
        }

        public async Task DeleteAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            await _userCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
