using Amazon.Runtime.Internal;
using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/")]
    public class UserController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public UserController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }


        [HttpGet]
        public async Task<List<User>> Get()
        {
            return await _mongoDBService.GetAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            await _mongoDBService.CreateAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _mongoDBService.FindUserById(id);

            if (user == null)
            {
                return NotFound(); // Обробка випадку, коли користувача не знайдено
            }

            return Ok(user);
        }
    }
}
