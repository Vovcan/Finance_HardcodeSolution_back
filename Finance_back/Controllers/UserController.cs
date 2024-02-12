using Amazon.Runtime.Internal;
using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("/users")]
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
            return await _mongoDBService.GetUsersAsync();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            await _mongoDBService.CreateUserAsync(user);
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _mongoDBService.FindUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(); // Обробка випадку, коли користувача не знайдено
            }

            return Ok(user);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(string id, [FromBody] User updatedUser)
        {
            if (updatedUser == null)
            {
                return BadRequest("Invalid user data");
            }

            updatedUser.Id = id; // Ensure the updatedUser has the correct Id

            var existingUser = await _mongoDBService.FindUserByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound(); // Handle the case where the user was not found
            }

            // Use a method to update only the non-null properties
            var updatedUserResult = await _mongoDBService.UpdateUserAsync(existingUser, updatedUser);

            return Ok(updatedUserResult);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _mongoDBService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
