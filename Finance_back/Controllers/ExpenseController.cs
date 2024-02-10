using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/expense")]
    public class ExpenseController : Controller
    {
        private readonly MongoDBService _mongoDBService;
        public ExpenseController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBService = new MongoDBService(mongoDBSettings);
        }
        [HttpGet]
        public async Task<List<Expense>> Get()
        {
            return await _mongoDBService.GetExpenseAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Expense _expense)
        {
            await _mongoDBService.CreateExpenseAsync(_expense);
            return CreatedAtAction(nameof(Get), new { id = _expense.Id }, _expense);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncome(string id)
        {
            var _incomeCategory = await _mongoDBService.FindExpenseByIdAsync(id);

            if (_incomeCategory == null)
            {
                return NotFound();
            }

            return Ok(_incomeCategory);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncome(string id, [FromBody] Expense updatedExpense)
        {
            if (updatedExpense == null)
            {
                return BadRequest("Invalid user data");
            }

            updatedExpense.Id = id; // Ensure the updatedUser has the correct Id

            var existingExpense = await _mongoDBService.FindExpenseByIdAsync(id);

            if (existingExpense == null)
            {
                return NotFound(); // Handle the case where the user was not found
            }

            // Use a method to update only the non-null properties
            var updatedUserResult = await _mongoDBService.UpdateExpenseAsync(existingExpense, updatedExpense);

            return Ok(updatedUserResult);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(string id)
        {
            await _mongoDBService.DeleteExpenseAsync(id);
            return NoContent();
        }
    }
}
