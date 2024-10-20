using Microsoft.AspNetCore.Mvc;

namespace SanThuongMaiG15.Areas.Seller.Controllers
{
    public class HomeController : Controller
    {
        [Area("Seller")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
