using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/expense-categorie")]
    public class ExpenseCategoryController : Controller
    {
        private readonly MongoDBService _mongoDBService;
        public ExpenseCategoryController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBService = new MongoDBService(mongoDBSettings);
        }
        [HttpGet]
        public async Task<List<ExpenseCategory>> Get()
        {
            return await _mongoDBService.GetExpenseCategoryAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExpenseCategory _expenseCategory)
        {
            await _mongoDBService.CreateExpenseCategoryAsync(_expenseCategory);
            return CreatedAtAction(nameof(Get), new { id = _expenseCategory.Id }, _expenseCategory);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncomeCategory(string id)
        {
            var _incomeCategory = await _mongoDBService.FindExpenseCategoryByIdAsync(id);

            if (_incomeCategory == null)
            {
                return NotFound(); // Обробка випадку, коли користувача не знайдено
            }

            return Ok(_incomeCategory);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncomeCategory(string id, [FromBody] ExpenseCategory updatedExpenseCategory)
        {
            if (updatedExpenseCategory == null)
            {
                return BadRequest("Invalid user data");
            }

            updatedExpenseCategory.Id = id; // Ensure the updatedUser has the correct Id

            var existingExpenseCategory = await _mongoDBService.FindExpenseCategoryByIdAsync(id);

            if (existingExpenseCategory == null)
            {
                return NotFound(); // Handle the case where the user was not found
            }

            // Use a method to update only the non-null properties
            var updatedUserResult = await _mongoDBService.UpdateExpenseCategoryAsync(existingExpenseCategory, updatedExpenseCategory);

            return Ok(updatedUserResult);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpenseCategoryAsync(string id)
        {
            await _mongoDBService.DeleteExpenseCategoryAsync(id);
            return NoContent();
        }
    }
}
