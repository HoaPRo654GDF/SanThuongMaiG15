using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SanThuongMaiG15.Models;
using SanThuongMaiG15.ModelViews;

namespace SanThuongMaiG15.Controllers
{
    public class OrdersController : Controller
    {
        private readonly EcC2CContext _context;

        public OrdersController(EcC2CContext context)
        {
            _context = context;
        }

        [Route("xem-don-hang.html", Name = "xemdonhang")]
        public async Task<IActionResult> Index()
        {
            //var taikhoanID = HttpContext.Session.GetString("UserId");
            var taikhoanID = User.FindFirstValue("UserID");

            int userId = Convert.ToInt32(taikhoanID);

            var ecC2CContext = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.TransactStatus)
                .Where(o => o.BuyerId == userId); 

            return View(await ecC2CContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taikhoanID = User.FindFirstValue("UserID");
            int userId = Convert.ToInt32(taikhoanID);

            // Thêm Include cho OrderDetails và Product
            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.TransactStatus)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) // Thêm vào để lấy thông tin sản phẩm
                .FirstOrDefaultAsync(m => m.OrderId == id && m.BuyerId == userId);

            if (order == null)
            {
                return NotFound();
            }

            var orderDetailViewModel = new OrderDetailViewModel()
            {
                OrderId = order.OrderId,
                OrderStatus = order.TransactStatus.Status,
                BuyerName = order.Buyer.Username,
                OrderDetails = order.OrderDetails.ToList()
            };

            return View(orderDetailViewModel);
        }


        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
