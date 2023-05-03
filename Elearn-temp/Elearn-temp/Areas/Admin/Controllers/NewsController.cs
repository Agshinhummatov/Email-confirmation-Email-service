using Elearn_temp.Data;
using Elearn_temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Elearn_temp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly AppDbContext _context;

        public NewsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<News> news = await _context.News.Where(m => !m.SoftDelete).ToListAsync();

            return View(news);
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {

            if (id == null) return BadRequest();


            News? news = await _context.News.Where(m => !m.SoftDelete).FirstOrDefaultAsync(m => m.Id == id);

            if (news is null) return NotFound();

            return View(news);

        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }




    }
}
