using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SanThuongMaiG15.Areas.Seller.Controllers
{
    public class HomeController : Controller
    {
        [Area("Seller")]
        [Authorize(Roles = "2")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
