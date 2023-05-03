using EntityFramework_Slider.Data;
using EntityFramework_Slider.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EntityFramework_Slider.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        public ShopController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {

            int count = await _context.Products.Where(m => !m.SoftDelete).CountAsync();

            ViewBag.Count = count;

            IEnumerable<Product> products = await _context.Products
                                                  .Include(m => m.Images)
                                                  .Where(m => !m.SoftDelete)
                                                  //.OrderByDescending(m => m.Id)  //Id sine gore datalari siralamaq uchun
                                                  //.Skip(4) /4 denesini kechib sonra kestermek uchun
                                                  .Take(4)   //4 datani gostermek uchun 
                                                  .ToListAsync();

            return View(products);
        }

        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Product> products = await _context.Products
                                               .Include(m => m.Images)
                                               .Where(m => !m.SoftDelete)
                                               //.OrderByDescending(m => m.Id)  //Id sine gore datalari siralamaq uchun
                                               //.Skip(4) /4 denesini kechib sonra kestermek uchun
                                               .Skip(skip)
                                               .Take(4)   //4 datani gostermek uchun 
                                               .ToListAsync();

            //partialview buna goredirki Partial -imiza bu product modelimizi qoyub hazirlayib  gonderirik jscripte.ichini doldurub gonderiri js e.
            return PartialView("_ProductsPartial", products);
        }



        public IActionResult Search(string searchText)
        {
            var products = _context.Products
                            .Include(m => m.Images)
                            .Include(m => m.Category)
                            .OrderByDescending(m => m.Id)    //best practice CreateDate e gore orderby etmek
                            .Where(m => !m.SoftDelete && m.Name.ToLower().Contains(searchText.ToLower()))
                            .Take(6)
                            .ToList();


            return PartialView("_SearchPartial", products);
        }
    }

}
