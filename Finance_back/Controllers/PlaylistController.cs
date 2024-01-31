using Amazon.Runtime.Internal;
using Finance_back.Models;
using Finance_back.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finance_back.Controllers
{
    [Controller]
    [Route("api/[controller]")]
    public class PlaylistController : Controller
    {
        private readonly MongoDBService _mongoDBService;

        public PlaylistController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpGet]
        public async Task<List<User>> Get() { 
            return await _mongoDBService.GetAsync();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User playlist) {
            await _mongoDBService.CreateAsync(playlist);
            return CreatedAtAction(nameof(Get), new { id = playlist.Id }, playlist);
        }

        //[HttpPut]
        //public async Task<IActionResult> AddToPlaylist(string id, [FromBody] string movieId)
        //{
        //    await _mongoDBService.AddToPlaylistAsync(id, movieId);
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id) {
        //    await _mongoDBService.DeleteAsync(id);
        //    return NoContent();
        //}
    }
}
