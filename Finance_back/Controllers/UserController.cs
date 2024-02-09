using Amazon.Runtime.Internal;
using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/user")]
    public class UserController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public UserController(IOptions<MongoDBSettings> mongoDBSettings)
        {
            _mongoDBService = new MongoDBService(mongoDBSettings);
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
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] User updatedUser)
        {
            updatedUser.Id = id; // Ensure the updatedUser has the correct Id

            var updatedUserResult = await _mongoDBService.UpdateUser(updatedUser);

            if (updatedUserResult != null)
            {
                return Ok(updatedUserResult);
            }

            return NotFound(); // Handle the case where the user was not found or no modifications were made
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteAsync(id);
            return NoContent();
        }
    }
}
