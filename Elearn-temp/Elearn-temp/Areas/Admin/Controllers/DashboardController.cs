using Microsoft.AspNetCore.Mvc;

namespace Elearn_temp.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Admin")]

        public IActionResult Index()
        {
            return View();
        }
    }
}
