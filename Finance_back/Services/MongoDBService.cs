using Finance_back.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Finance_back.Services

{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<IncomeCategory> _IncomeCategoryCollection;
        //conect to database
        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>("users");
            _IncomeCategoryCollection = database.GetCollection<IncomeCategory>("IncomeCategorys");
        }
        // -----  User Functions
        //create new user
        public async Task CreateUserAsync(User user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            await _userCollection.InsertOneAsync(user);
            return;
        }
        //get for all users
        public async Task<List<User>> GetUsersAsync()
        {
            return await _userCollection.Find(new BsonDocument()).ToListAsync();
        }
        //get 1 user by id
        public async Task<User> FindUserByIdAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            User user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
        public async Task<User> UpdateUserAsync(User existingUser, User updatedUser)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, existingUser.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<User>.Update
                .Set(u => u.Name, updatedUser.Name ?? existingUser.Name)
                .Set(u => u.Email, updatedUser.Email ?? existingUser.Email)
                .Set(u => u.Password, updatedUser.Password ?? existingUser.Password)
                .Set(u => u.Pin, updatedUser.Pin ?? existingUser.Pin);
         // Add similar lines for other properties

    var result = await _userCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingUser;
            }

            return null;
        }

        public async Task DeleteUserAsync(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq("Id", id);
            await _userCollection.DeleteOneAsync(filter);
            return;
        }
        // -----  IncomeCategory Functions
        public async Task CreateIncomeCategoryAsync(IncomeCategory incomeCategory)
        {
            await _IncomeCategoryCollection.InsertOneAsync(incomeCategory);
            return;
        }
        public async Task<List<IncomeCategory>> GetIncomeCategoryAsync()
        {
            return await _IncomeCategoryCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<IncomeCategory> FindIncomeCategoryByIdAsync(string id)
        {
            FilterDefinition<IncomeCategory> filter = Builders<IncomeCategory>.Filter.Eq("Id", id);
            IncomeCategory user = await _IncomeCategoryCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }

        public async Task<IncomeCategory> UpdateIncomeCategoryAsync(IncomeCategory existingIncomeCategory, IncomeCategory updatedIncomeCategory)
        {
            var filter = Builders<IncomeCategory>.Filter.Eq(u => u.Id, existingIncomeCategory.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<IncomeCategory>.Update
                .Set(u => u.Name, updatedIncomeCategory.Name ?? existingIncomeCategory.Name)
                .Set(u => u.Sum, updatedIncomeCategory.Sum ?? existingIncomeCategory.Sum)
                .Set(u => u.UserId, updatedIncomeCategory.UserId ?? existingIncomeCategory.UserId);
            // Add similar lines for other properties

            var result = await _IncomeCategoryCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingIncomeCategory;
            }

            return null;
        }
        public async Task DeleteIncomeCategoryAsync(string id)
        {
            FilterDefinition<IncomeCategory> filter = Builders<IncomeCategory>.Filter.Eq("Id", id);
            await _IncomeCategoryCollection.DeleteOneAsync(filter);
            return;
        }
    }
}
