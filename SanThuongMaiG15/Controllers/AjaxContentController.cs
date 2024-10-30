using Microsoft.AspNetCore.Mvc;

namespace SanThuongMaiG15.Controllers
{
    public class AjaxContentController : Controller
    {
        public IActionResult HeaderCart()
        {
            return ViewComponent("HeaderCart");
        }
        public IActionResult HeaderFavourites()
        {
            return ViewComponent("NumberCart");
        }
    }
}
