using SanThuongMaiG15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PagedList.Core;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SanThuongMaiG15.Controllers
{
    public class ProductController : Controller
    {
        private readonly  EcC2C_2Context _context;

        public ProductController(EcC2C_2Context context)
        {

            _context = context;

        }

        [Route("shop.html", Name = "ShopProduct")]
        public IActionResult Index(int? page)
        {
            try
            {
                var pageNumber = page == null || page <= 0 ? 1 : page.Value;
                var pageSize = 10;
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .OrderByDescending(x => x.DatePosted);
                PagedList<Product> models = new PagedList<Product>(lsProduct.AsQueryable(), pageNumber, pageSize);
                ViewBag.CurrentPage = pageNumber;
                return View(models);
            }
            catch
            {

                if (User.Identity.IsAuthenticated)
                {
                    // Lấy RoleId của người dùng
                    var roleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Chuyển hướng dựa trên RoleId
                    if (roleId == "3")
                    {
                        return Redirect("/Admin/Home/Index");
                    }
                    else if (roleId == "2")
                    {
                        return Redirect("/Seller/Home/Index");
                    }

                }
                return RedirectToAction("Index", "Home");
            }
           
           
            
        }


        [Route("{CatName}", Name = "ListProduct")]
        public IActionResult List(string CatName, int page=1)
        {
            try
            {
                var pageSize = 10;
                var danhmuc = _context.Categories.AsNoTracking().SingleOrDefault(x => x.CatName == CatName);
                var lsProduct = _context.Products
                    .AsNoTracking()
                    .Where(x => x.CatId == danhmuc.CatId)
                    .OrderByDescending(x => x.DatePosted);
                PagedList<Product> models = new PagedList<Product>(lsProduct.AsQueryable(), page, pageSize);
                ViewBag.CurrentPage = page;
                ViewBag.CurrentCat = danhmuc;
                return View(models);
            }
            catch
            {
               
                if (User.Identity.IsAuthenticated)
                {
                    // Lấy RoleId của người dùng
                    var roleId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Chuyển hướng dựa trên RoleId
                    if (roleId == "3") // Nếu là admin
                    {
                        return Redirect("/Admin/Home/Index"); // Chuyển đến trang admin
                    }
                    else if (roleId == "2") // Nếu là seller
                    {
                        return Redirect("/Seller/Home/Index"); // Chuyển đến trang seller
                    }


                }
                return RedirectToAction("Index", "Home");
            }
                
          

           
        }


        [Route("/{ProductName}-{id}.html", Name= "ProductDetails")]
        public IActionResult Details(int id)
        {
          
                var product = _context.Products.Include(x => x.Cat).FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {

                    return RedirectToAction("Index");

                }
                //lấy 4 product tương đồng
                var lsProduct = _context.Products.AsNoTracking()
                    .Where(x => x.CatId == product.CatId && x.ProductId != id)
                    .OrderByDescending(x=>x.DatePosted)
                    .Take(4)
                    .ToList();

                ViewBag.SanPham = lsProduct;
                return View(product);
          
            
        }

    
    }
}
