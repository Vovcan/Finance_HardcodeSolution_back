using System.Collections.Generic;
using Finance_back.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Finance_back.Services

{
    public class MongoDBService
    {
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<IncomeCategory> _IncomeCategoryCollection;
        private readonly IMongoCollection<Income> _IncomeCollection;
        private readonly IMongoCollection<ExpenseCategory> _ExpenseCategoryCollection;
        private readonly IMongoCollection<Expense> _ExpenseCollection;
        private readonly IMongoCollection<Reminder> _ReminderCollection;
        //connection to database
        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _userCollection = database.GetCollection<User>("users");
            _IncomeCategoryCollection = database.GetCollection<IncomeCategory>("IncomeCategorys");
            _IncomeCollection = database.GetCollection<Income>("Incomes");
            _ExpenseCategoryCollection = database.GetCollection<ExpenseCategory>("ExpenseCategory");
            _ExpenseCollection = database.GetCollection<Expense>("Expense");
            _ReminderCollection = database.GetCollection<Reminder>("Reminder");
        }
        // -----  User Functions
        public async Task CreateUserAsync(User user)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = hashedPassword;
            await _userCollection.InsertOneAsync(user);
            return;
        }
        public async Task<List<User>> GetUsersAsync()
        {
            return await _userCollection.Find(new BsonDocument()).ToListAsync();
        }
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

         //-----  IncomeCategory Functions
        public async Task CreateIncomeCategoryAsync(IncomeCategory incomeCategory)
        {
            incomeCategory.Sum = 0;
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
            IncomeCategory _incomeCategory = await _IncomeCategoryCollection.Find(filter).FirstOrDefaultAsync();
            return _incomeCategory;
        }
        public async Task<IncomeCategory> UpdateIncomeCategoryAsync(IncomeCategory existingIncomeCategory, IncomeCategory updatedIncomeCategory)
        {
            var filter = Builders<IncomeCategory>.Filter.Eq(u => u.Id, existingIncomeCategory.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<IncomeCategory>.Update
                .Set(u => u.Name, updatedIncomeCategory.Name ?? existingIncomeCategory.Name)
                .Set(u => u.UserId, updatedIncomeCategory.UserId ?? existingIncomeCategory.UserId);
            // Add similar lines for other properties

            var result = await _IncomeCategoryCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingIncomeCategory;
            }
            return null;
        }
        public async Task<IncomeCategory> UpdateSumIncomeCategoryAsync(IncomeCategory existingIncomeCategory)
        {

            // Фільтруємо всі записи Income, де IncomeCategory дорівнює вказаному categoryId
            var filter = Builders<Income>.Filter.Eq(x => x.IncomeCategory, existingIncomeCategory.Id);

            // Вибираємо поле Amount для всіх відфільтрованих записів
            var projection = Builders<Income>.Projection.Expression(x => x.Amount);

            // Використовуємо LINQ для обчислення суми
            var totalAmount = _IncomeCollection.Find(filter).Project(projection).ToEnumerable().Sum();

            var _filter = Builders<IncomeCategory>.Filter.Eq(u => u.Id, existingIncomeCategory.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<IncomeCategory>.Update
                .Set(u => u.Id,  existingIncomeCategory.Id)
                .Set(u => u.Name,  existingIncomeCategory.UserId)
                .Set(u => u.Sum, totalAmount)
                .Set(u => u.UserId, existingIncomeCategory.UserId);
            // Add similar lines for other properties

            var result = await _IncomeCategoryCollection.UpdateOneAsync(_filter, updateDefinition);

            return null;
        }
        public async Task DeleteIncomeCategoryAsync(string id)
        {
            FilterDefinition<IncomeCategory> filter = Builders<IncomeCategory>.Filter.Eq("Id", id);
            await _IncomeCategoryCollection.DeleteOneAsync(filter);
            return;
        }

        // -----  Income Functions
        public async Task CreateIncomeAsync(Income income)
        {
            await _IncomeCollection.InsertOneAsync(income);
            return;
        }
        public async Task<List<Income>> GetIncomeAsync()
        {
            return await _IncomeCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<Income> FindIncomeByIdAsync(string id)
        {
            FilterDefinition<Income> filter = Builders<Income>.Filter.Eq("Id", id);
            Income user = await _IncomeCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
        public async Task<Income> UpdateIncomeAsync(Income existingIncome, Income updatedIncome)
        {
            var filter = Builders<Income>.Filter.Eq(u => u.Id, existingIncome.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<Income>.Update
                .Set(u => u.Name, updatedIncome.Name ?? existingIncome.Name)
                .Set(u => u.Amount, updatedIncome.Amount ?? existingIncome.Amount)
                .Set(u => u.IncomeCategory, updatedIncome.IncomeCategory ?? existingIncome.IncomeCategory)
                .Set(u => u.UserId, updatedIncome.UserId ?? existingIncome.UserId);
            // Add similar lines for other properties

            var result = await _IncomeCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingIncome;
            }
            return null;
        }
        public async Task DeleteIncomeAsync(string id)
        {
            FilterDefinition<Income> filter = Builders<Income>.Filter.Eq("Id", id);
            await _IncomeCollection.DeleteOneAsync(filter);
            return;
        }

        //-----  ExpenseCategory Functions
        public async Task CreateExpenseCategoryAsync(ExpenseCategory expenseCategory)
        {
            expenseCategory.Sum = 0;
            await _ExpenseCategoryCollection.InsertOneAsync(expenseCategory);
            return;
        }
        public async Task<List<ExpenseCategory>> GetExpenseCategoryAsync()
        {
            return await _ExpenseCategoryCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<ExpenseCategory> FindExpenseCategoryByIdAsync(string id)
        {
            FilterDefinition<ExpenseCategory> filter = Builders<ExpenseCategory>.Filter.Eq("Id", id);
            ExpenseCategory user = await _ExpenseCategoryCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
        public async Task<ExpenseCategory> UpdateExpenseCategoryAsync(ExpenseCategory existingIncomeCategory, ExpenseCategory updatedIncomeCategory)
        {
            var filter = Builders<ExpenseCategory>.Filter.Eq(u => u.Id, existingIncomeCategory.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<ExpenseCategory>.Update
                .Set(u => u.Name, updatedIncomeCategory.Name ?? existingIncomeCategory.Name)
                .Set(u => u.UserId, updatedIncomeCategory.UserId ?? existingIncomeCategory.UserId);
            // Add similar lines for other properties

            var result = await _ExpenseCategoryCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingIncomeCategory;
            }
            return null;
        }
        public async Task DeleteExpenseCategoryAsync(string id)
        {
            FilterDefinition<ExpenseCategory> filter = Builders<ExpenseCategory>.Filter.Eq("Id", id);
            await _ExpenseCategoryCollection.DeleteOneAsync(filter);
            return;
        }
        public async Task<ExpenseCategory> UpdateSumExpenseCategoryAsync(ExpenseCategory existingExpenseCategory)
        {

            // Фільтруємо всі записи Income, де IncomeCategory дорівнює вказаному categoryId
            var filter = Builders<Expense>.Filter.Eq(x => x.ExpenseCategory, existingExpenseCategory.Id);

            // Вибираємо поле Amount для всіх відфільтрованих записів
            var projection = Builders<Expense>.Projection.Expression(x => x.Amount);

            // Використовуємо LINQ для обчислення суми
            var totalAmount = _ExpenseCollection.Find(filter).Project(projection).ToEnumerable().Sum();

            var _filter = Builders<ExpenseCategory>.Filter.Eq(u => u.Id, existingExpenseCategory.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<ExpenseCategory>.Update
                .Set(u => u.Id, existingExpenseCategory.Id)
                .Set(u => u.Name, existingExpenseCategory.UserId)
                .Set(u => u.Sum, totalAmount)
                .Set(u => u.UserId, existingExpenseCategory.UserId);
            // Add similar lines for other properties

            var result = await _ExpenseCategoryCollection.UpdateOneAsync(_filter, updateDefinition);

            return null;
        }

        // -----  Expense Functions
        public async Task CreateExpenseAsync(Expense expense)
        {
            await _ExpenseCollection.InsertOneAsync(expense);
            return;
        }
        public async Task<List<Expense>> GetExpenseAsync()
        {
            return await _ExpenseCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<Expense> FindExpenseByIdAsync(string id)
        {
            FilterDefinition<Expense> filter = Builders<Expense>.Filter.Eq("Id", id);
            Expense user = await _ExpenseCollection.Find(filter).FirstOrDefaultAsync();
            return user;
        }
        public async Task<Expense> UpdateExpenseAsync(Expense existingIncome, Expense updatedIncome)
        {
            var filter = Builders<Expense>.Filter.Eq(u => u.Id, existingIncome.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<Expense>.Update
                .Set(u => u.Name, updatedIncome.Name ?? existingIncome.Name)
                .Set(u => u.Amount, updatedIncome.Amount ?? existingIncome.Amount)
                .Set(u => u.ExpenseCategory, updatedIncome.ExpenseCategory ?? existingIncome.ExpenseCategory)
                .Set(u => u.UserId, updatedIncome.UserId ?? existingIncome.UserId);
            // Add similar lines for other properties

            var result = await _ExpenseCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingIncome;
            }
            return null;
        }
        public async Task DeleteExpenseAsync(string id)
        {
            FilterDefinition<Expense> filter = Builders<Expense>.Filter.Eq("Id", id);
            await _ExpenseCollection.DeleteOneAsync(filter);
            return;
        }

        // -----  Reaminder Functions
        public async Task CreateReminderAsync(Reminder reminder)
        {
            await _ReminderCollection.InsertOneAsync(reminder);
            return;
        }
        public async Task<List<Reminder>> GetReminderAsync()
        {
            return await _ReminderCollection.Find(new BsonDocument()).ToListAsync();
        }
        public async Task<Reminder> FindReminderByIdAsync(string id)
        {
            FilterDefinition<Reminder> filter = Builders<Reminder>.Filter.Eq("Id", id);
            Reminder _reminder = await _ReminderCollection.Find(filter).FirstOrDefaultAsync();
            return _reminder;
        }
        public async Task<List<Reminder>> GetRemindersForUserAsync(string UserId)
        {
            FilterDefinition<Reminder> filter = Builders<Reminder>.Filter.Eq("UserId", UserId);
            List<Reminder> _reminders = await _ReminderCollection.Find(filter).ToListAsync();
            return _reminders;
        }
        public async Task<Reminder> UpdateReminderAsync(Reminder existingReminder, Reminder updatedReminder)
        {
            var filter = Builders<Reminder>.Filter.Eq(u => u.Id, existingReminder.Id);

            // Use a projection to get only the non-null properties from updatedUser
            var updateDefinition = Builders<Reminder>.Update
                .Set(u => u.Title, updatedReminder.Title ?? existingReminder.Title)
                .Set(u => u.Description, updatedReminder.Description ?? existingReminder.Description)
                .Set(u => u.DueDate, updatedReminder.DueDate ?? existingReminder.DueDate);
            // Add similar lines for other properties

            var result = await _ReminderCollection.UpdateOneAsync(filter, updateDefinition);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
            {
                return existingReminder;
            }
            return null;
        }
        public async Task DeleteReminderAsync(string id)
        {
            FilterDefinition<Reminder> filter = Builders<Reminder>.Filter.Eq("Id", id);
            await _ReminderCollection.DeleteOneAsync(filter);
            return;
        }
        //public async Task<List<Reminder>> GetRemindersDueNow()
        //{

        //    DateTime date;
        //    DateTime now = DateTime.Now;
        //    now = now.AddSeconds(-now.Second).AddMilliseconds(-now.Millisecond);
        //    var filter = Builders<Reminder>.Filter.Empty;
        //    var reminders = _ReminderCollection.Find(filter).ToList();
        //    foreach (Reminder reminder in reminders )
        //    {
        //        date = reminder.DueDate;
        //        date = date.AddSeconds(-yourDate.Second).AddMilliseconds(-yourDate.Millisecond);
        //    }
        //    return null;
        //}


    }
}
