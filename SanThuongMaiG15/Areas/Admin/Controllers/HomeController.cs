using Microsoft.AspNetCore.Mvc;

namespace SanThuongMaiG15.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
