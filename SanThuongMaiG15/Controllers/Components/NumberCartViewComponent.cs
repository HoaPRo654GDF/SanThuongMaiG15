using Microsoft.AspNetCore.Mvc;
using SanThuongMaiG15.Extension;
using SanThuongMaiG15.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanThuongMaiG15.Controllers.Components
{
    public class NumberCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            if (cart == null || !cart.Any())
            {
                // In ra console hoặc log để biết là giỏ hàng trống
                Console.WriteLine("Giỏ hàng trống hoặc không có dữ liệu");
            }

            return View(cart);
        }
    }
}
