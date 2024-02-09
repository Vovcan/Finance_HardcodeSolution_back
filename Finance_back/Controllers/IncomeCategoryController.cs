using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/income-categorie")]
    public class IncomeCategoryController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public IncomeCategoryController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBService = new MongoDBService(mongoDBSettings);
        }
        [HttpGet]
        public async Task<List<IncomeCategory>> Get()
        {
            return await _mongoDBService.GetIncomeCategoryAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IncomeCategory _incomeCategory)
        {
            await _mongoDBService.CreateIncomeCategoryAsync(_incomeCategory);
            return CreatedAtAction(nameof(Get), new { id = _incomeCategory.Id }, _incomeCategory);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncomeCategory(string id)
        {
            var _incomeCategory = await _mongoDBService.FindIncomeCategoryByIdAsync(id);

            if (_incomeCategory == null)
            {
                return NotFound(); // Обробка випадку, коли користувача не знайдено
            }

            return Ok(_incomeCategory);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] IncomeCategory updatedIncomeCategory)
        {
            if (updatedIncomeCategory == null)
            {
                return BadRequest("Invalid user data");
            }

            updatedIncomeCategory.Id = id; // Ensure the updatedUser has the correct Id

            var existingIncomeCategory = await _mongoDBService.FindIncomeCategoryByIdAsync(id);

            if (existingIncomeCategory == null)
            {
                return NotFound(); // Handle the case where the user was not found
            }

            // Use a method to update only the non-null properties
            var updatedUserResult = await _mongoDBService.UpdateIncomeCategoryAsync(existingIncomeCategory, updatedIncomeCategory);

            return Ok(updatedUserResult);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteIncomeCategoryAsync(id);
            return NoContent();
        }
    }
}
