using Microsoft.AspNetCore.Mvc;

namespace ContactApp.Controllers
{
    [Route("news")]
    public class NewsController : Controller
    {
        
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var news= await _newsService.GetAllAsync(cancellationToken);
            return View(news);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken) {
            var newItem=await _newsService.GetByIdAsync(id,cancellationToken);
            if (newItem == null)
                return NotFound();
            return View(newItem);
        }


    }
}
