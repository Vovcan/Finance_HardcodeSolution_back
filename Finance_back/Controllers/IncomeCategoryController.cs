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
            return await _mongoDBService.GetAsyn();
        }
    }
}
