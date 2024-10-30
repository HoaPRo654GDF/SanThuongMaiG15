using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SanThuongMaiG15.Extension;
using SanThuongMaiG15.Models;
using SanThuongMaiG15.ModelViews;
using System.Collections.Generic;
using System.Linq;

namespace SanThuongMaiG15.Controllers
{
    public class CartController : Controller
    {
        private readonly EcC2CContext _context;
        public INotyfService _notifyService { get; }
        public CartController(EcC2CContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;

        }

        public List<CartItem> GioHang
        {
            get
            {

                var gh = HttpContext.Session.Get<List<CartItem>>("GioHang");
                if (gh == default(List<CartItem>))
                {
                    gh = new List<CartItem>();
                }

                return gh;
            }
        }

        [Route("Cart.html", Name = "Gio Hang")]
        public IActionResult Index()
        {


            return View(GioHang);
        }
        //[HttpPost]
        //[Route("api/cart/add")]
        //public IActionResult AddToCart(int productID, int? quantity)
        //{
        //    try
        //    {
        //        List<CartItem> gioHang = GioHang;

        //        CartItem item = GioHang.SingleOrDefault(p => p.product.ProductId == productID);
        //        if (item != null)
        //        {
        //            if (quantity.HasValue)
        //            {
        //                item.quantity = quantity.Value;
        //            }
        //            else
        //            {
        //                item.quantity++;
        //            }
        //        }
        //        else
        //        {
        //            Product gh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
        //            item = new CartItem
        //            {
        //                quantity = quantity.HasValue ? quantity.Value : 1,
        //                product = gh
        //            };
        //            gioHang.Add(item);
        //        }
        //        HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
        //        return Json(new { success = true });
        //    }
        //    catch
        //    {
        //        return Json(new { success = false });
        //    }
        //}
        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? quantity)
        {
            try
            {
                List<CartItem> gioHang = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();

                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    
                    item.quantity += quantity ?? 1; 
                }
                else
                {
                    
                    Product gh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItem
                    {
                        quantity = quantity ?? 1, 
                        product = gh
                    };
                    gioHang.Add(item);
                }

            
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? quantity)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItem item = cart.SingleOrDefault(p => p.product.ProductId == productID);
                    if (item != null && quantity.HasValue)
                    {
                        item.quantity = quantity.Value;
                    }
                    HttpContext.Session.Set<List<CartItem>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

            [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {
            try 
            {
                List<CartItem> gioHang = GioHang;
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                return Json(new { success = true });
            } 
            catch 
            {
                return Json(new { success = false });
            }
        }
            
    }
}

