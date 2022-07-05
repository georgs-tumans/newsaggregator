using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace helmesproject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        
        private readonly AppDbContext _dbContext;
        private readonly ILogger<NewsController> _logger;

        public NewsController(ILogger<NewsController> logger, AppDbContext context)
        {
            _logger = logger;
            _dbContext = context;
        }

        [HttpGet]
        public async Task<IEnumerable<NewsItem>> GetAsync()
        {

            return await _dbContext.NewsItems.ToListAsync();

        }

    }
}