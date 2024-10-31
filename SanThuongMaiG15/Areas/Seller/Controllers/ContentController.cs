using Microsoft.AspNetCore.Mvc;

namespace SanThuongMaiG15.Areas.Seller.Controllers
{
    public class ContentController : Controller
    {
        public IActionResult NameUser()
        {
            return ViewComponent("NameUser");
        }
    }
}
