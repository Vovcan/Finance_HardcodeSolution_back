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
    }
}
