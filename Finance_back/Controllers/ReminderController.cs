using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/reminders")]
    public class ReminderController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public ReminderController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBService = new MongoDBService(mongoDBSettings);
        }
        [HttpGet]
        public async Task<List<Reminder>> Get()
        {
            return await _mongoDBService.GetReminderAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Reminder reminder)
        {
            await _mongoDBService.CreateReminderAsync(reminder);
            return CreatedAtAction(nameof(Get), new { id = reminder.Id }, reminder);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReminder(string id)
        {
            var _Reminder = await _mongoDBService.FindReminderByIdAsync(id);

            if (_Reminder == null)
            {
                return NotFound();
            }

            return Ok(_Reminder);
        }
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetRemindersForUser(string userId)
        {
            // Логіка для отримання нагадувань для конкретного користувача (за userId)
            var reminders = await _mongoDBService.GetRemindersForUserAsync(userId);

            if (reminders == null || reminders.Count == 0)
            {
                return NotFound(); // Можна також повернути NoContent(), залежно від ваших потреб
            }

            return Ok(reminders);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] Reminder updatedReminder)
        {
            if (updatedReminder == null)
            {
                return BadRequest("Invalid user data");
            }

            updatedReminder.Id = id; // Ensure the updatedUser has the correct Id

            var existingIncome = await _mongoDBService.FindReminderByIdAsync(id);
            if (existingIncome == null)
            {
                return NotFound(); // Handle the case where the user was not found
            }

            // Use a method to update only the non-null properties
            var updatedReminderResult = await _mongoDBService.UpdateReminderAsync(existingIncome, updatedReminder);
            return Ok(updatedReminderResult);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReminder(string id)
        {
            await _mongoDBService.DeleteReminderAsync(id);
            return NoContent();
        }
    }
}
