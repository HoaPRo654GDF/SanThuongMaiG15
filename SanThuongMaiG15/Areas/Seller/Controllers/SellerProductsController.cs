using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using SanThuongMaiG15.Models;

namespace SanThuongMaiG15.Areas.Seller.Controllers
{
    [Area("Seller")]
    [Authorize(Roles ="2")]
    public class SellerProductsController : Controller
    {
        private readonly EcC2CContext _context;

        public SellerProductsController(EcC2CContext context)
        {
            _context = context;
        }

        // GET: Seller/SellerProducts
        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var pageNumber = page;

            var pageSize = 20;

            //var sellerID = HttpContext.Session.GetInt32("SellerID");
            //if (sellerID == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            //Console.WriteLine($"Authenticated SellerID: {sellerID}");


            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Console.WriteLine($"Authenticated User: {User.Identity.Name}");
            }

            // Lấy tên người dùng đã đăng nhập
            var email = User.Identity.Name;

            // Tìm người dùng trong cơ sở dữ liệu để lấy SellerID
            var seller = _context.Users.FirstOrDefault(u => u.Email == email);
            Console.WriteLine($"Email: {email}");
            Console.WriteLine($"Seller Found: {seller?.UserId}, RoleID: {seller?.RoleId}");
            if (seller == null)
            {

                return NotFound("Người dùng không tồn tại.");
            }
            List<Product> lsProducts = new List<Product>();
            lsProducts = _context.Products
             .AsNoTracking()
             .Include(x => x.Cat)
             //.Where(x => x.SellerId == sellerID)
             .Where(x => x.SellerId == seller.UserId)
             .OrderByDescending(x => x.ProductId)
             .ToList();
            if (CatID != 0)
            {
                lsProducts = lsProducts.Where(x => x.CatId == CatID).ToList();
            }
            Console.WriteLine($"So luong san pham: {lsProducts.Count}");
            PagedList<Product> models = new PagedList<Product>(lsProducts.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.currentPage = pageNumber;


            ViewData["DanhMuc"] = new SelectList(_context.Categories, "CatId", "CatName", CatID);

            return View(models);

        }

        // GET: Seller/SellerProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Seller/SellerProducts/Create
        public IActionResult Create()
        {
            

            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName");
            ViewData["SellerId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: Seller/SellerProducts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,CatId,Price,Quantity,SellerId,DatePosted,ImageUrl,ProductStatus,Thumb")] Product product)
        {
            //var sellerID = HttpContext.Session.GetInt32("SellerID");

            //if (sellerID != null)
            //{
            //    product.SellerId = sellerID.Value;
            //}
            var email = User.Identity.Name;
            var seller = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (seller != null)
            {
                product.SellerId = seller.UserId;
            }
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["SellerId"] = new SelectList(_context.Users, "UserId", "Email", product.SellerId);
            return View(product);
        }

        // GET: Seller/SellerProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["SellerId"] = new SelectList(_context.Users, "UserId", "Email", product.SellerId);
            return View(product);
        }

        // POST: Seller/SellerProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Description,CatId,Price,Quantity,SellerId,DatePosted,ImageUrl,ProductStatus,Thumb")] Product product)
        {
            //var sellerID = HttpContext.Session.GetInt32("SellerID");

            //if (sellerID != null)
            //{
            //    product.SellerId = sellerID.Value;
            //}
            var email = User.Identity.Name;
            var seller = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (seller != null)
            {
                product.SellerId = seller.UserId;
            }
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", product.CatId);
            ViewData["SellerId"] = new SelectList(_context.Users, "UserId", "Email", product.SellerId);
            return View(product);
        }

        // GET: Seller/SellerProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Cat)
                .Include(p => p.Seller)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Seller/SellerProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
