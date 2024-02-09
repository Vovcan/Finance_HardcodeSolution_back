using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/incomes")]
    public class IncomeController : Controller
    {
        private readonly MongoDBService _mongoDBService;
        public IncomeController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBService = new MongoDBService(mongoDBSettings);
        }
        [HttpGet]
        public async Task<List<Income>> Get()
        {
            return await _mongoDBService.GetIncomeAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Income _income)
        {
            await _mongoDBService.CreateIncomeAsync(_income);
            return CreatedAtAction(nameof(Get), new { id = _income.Id }, _income);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncome(string id)
        {
            var _incomeCategory = await _mongoDBService.FindIncomeByIdAsync(id);

            if (_incomeCategory == null)
            {
                return NotFound();
            }

            return Ok(_incomeCategory);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncome(string id, [FromBody] Income updatedIncome)
        {
            if (updatedIncome == null)
            {
                return BadRequest("Invalid user data");
            }

            updatedIncome.Id = id; // Ensure the updatedUser has the correct Id

            var existingIncome = await _mongoDBService.FindIncomeByIdAsync(id);

            if (existingIncome == null)
            {
                return NotFound(); // Handle the case where the user was not found
            }

            // Use a method to update only the non-null properties
            var updatedUserResult = await _mongoDBService.UpdateIncomeAsync(existingIncome, updatedIncome);

            return Ok(updatedUserResult);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncome(string id)
        {
            await _mongoDBService.DeleteIncomeAsync(id);
            return NoContent();
        }
    }
}
