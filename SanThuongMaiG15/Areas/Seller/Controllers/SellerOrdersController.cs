using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using SanThuongMaiG15.Models;

namespace SanThuongMaiG15.Areas.Seller.Controllers
{
    [Area("Seller")]
    public class SellerOrdersController : Controller
    {
        private readonly EcC2C_2Context _context;

        public SellerOrdersController(EcC2C_2Context context)
        {
            _context = context;
        }

        // GET: Seller/SellerOrders
        public IActionResult Index(int? page )
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;

            var pageSize = 20;

          
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Console.WriteLine($"Authenticated User: {User.Identity.Name}");
            }

            
            var email = User.Identity.Name;

            var seller = _context.Users.FirstOrDefault(u => u.Email == email);

            

       

            var Orders = _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.TransactStatus)
                .Include(o => o.OrderDetails) // Bao gồm chi tiết đơn hàng
                .ThenInclude(od => od.Product) // Bao gồm sản phẩm
                .AsNoTracking()
                .Where(o => o.OrderDetails.Any(od => od.Product.SellerId == seller.UserId)) // Điều kiện lọc
                .OrderByDescending(o => o.OrderDate);

            PagedList<Order> models = new PagedList<Order>(Orders, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;

            return View(models);

        }

        // GET: Seller/SellerOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }
            var Chitietdonhang = _context.OrderDetails
                .Include(o => o.Product)
                .AsNoTracking()
                .Where(x => x.OrderId == order.OrderId)
                .OrderBy(x=>x.OrderDetailId).ToList();
            ViewBag.ChiTiet = Chitietdonhang;
            return View(order);
        }

        // GET: Seller/SellerOrders/Create
        public IActionResult Create()
        {
            ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "Email");
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status");
            return View();
        }

        // POST: Seller/SellerOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,BuyerId,OrderDate,TransactStatusId,TotalMoney,Note,Address,PaymentId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "Email", order.BuyerId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }


        // GET: Seller/SellerOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "Email", order.BuyerId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }


        // POST: Seller/SellerOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,BuyerId,OrderDate,TransactStatusId,TotalMoney,Note,Address,PaymentId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuyerId"] = new SelectList(_context.Users, "UserId", "Email", order.BuyerId);
            ViewData["TransactStatusId"] = new SelectList(_context.TransactStatuses, "TransactStatusId", "Status", order.TransactStatusId);
            return View(order);
        }

        // GET: Seller/SellerOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Buyer)
                .Include(o => o.TransactStatus)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Seller/SellerOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
