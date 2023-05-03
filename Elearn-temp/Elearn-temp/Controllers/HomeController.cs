
using Elearn_temp.Data;
using Elearn_temp.Models;
using Elearn_temp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;


namespace Elearn_temp.Controllers
{
    public class HomeController : Controller
    {

        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task <IActionResult> Index()
        {
            List<Slider> sliders = await _context.Sliders.Where(m => !m.SoftDelete).ToListAsync();


            IEnumerable<Author> authors = await _context.Authors.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Course> courses = await _context.Courses.Include(m => m.CourseImages).Include(m => m.Author).Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<Event> events = await _context.Events.Where(m => !m.SoftDelete).ToListAsync();

            IEnumerable<News> news = await _context.News.Where(m => !m.SoftDelete).ToListAsync();

            HomeVM model = new ()
            {

                Sliders = sliders,
                Authors = authors,
                Courses = courses,
                Events = events,
                News = news

            };

            return View(model);

        }

     

       
    }
}