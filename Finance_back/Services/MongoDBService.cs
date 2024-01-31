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

        //public async Task AddToPlaylistAsync(string id, string movieId)
        //{
        //    FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("Id", id);
        //    UpdateDefinition<Playlist> update = Builders<Playlist>.Update.AddToSet<string>("items", movieId);
        //    await _playlistCollection.UpdateOneAsync(filter, update);
        //    return;
        //}

        //public async Task DeleteAsync(string id)
        //{
        //    FilterDefinition<Playlist> filter = Builders<Playlist>.Filter.Eq("Id", id);
        //    await _playlistCollection.DeleteOneAsync(filter);
        //    return;
        //}
    }
}
