using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SanThuongMaiG15.Extension;
using SanThuongMaiG15.Models;
using SanThuongMaiG15.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SanThuongMaiG15.Controllers
{
    public class AccountsController : Controller
    {
        private readonly EcC2CContext _context;
        public INotyfService _notifyService { get; }

        public AccountsController(EcC2CContext context, INotyfService notifyService)
        {
            _context = context;
            _notifyService = notifyService;
        }

       
        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        [Authorize]
        public IActionResult Dashboard()
        {
            // Kiểm tra xem người dùng đã xác thực hay chưa
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            // Lấy ID người dùng từ claims
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (int.TryParse(userIdClaim, out int taikhoanID))
            {
                var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == taikhoanID);
                if (khachhang != null)
                {
                    return View(khachhang); // Trả về view với thông tin tài khoản
                }
            }

            // Nếu không tìm thấy người dùng, chuyển hướng về trang đăng nhập
            return RedirectToAction("Login");
        }

       
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // Nếu đã đăng nhập, chuyển hướng đến Dashboard
                return RedirectToAction("Dashboard", "Accounts");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(LoginViewModel user, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            var khachhang = await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Email.Trim() == user.Email.Trim());

            if (khachhang == null || khachhang.Password != user.Password.ToMD5())
            {
                _notifyService.Error("Thông tin đăng nhập không hợp lệ!");
                return View(user);
            }

            // Tạo Claims và đăng nhập người dùng
            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Email, khachhang.Email),
                new Claim("UserID", khachhang.UserId.ToString()),
                new Claim(ClaimTypes.Role, khachhang.RoleId.ToString())
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            _notifyService.Success("Đăng nhập thành công!");

            //switch (khachhang.RoleId)
            //{
            //    case 3: // Admin
            //        return RedirectToAction("Index", "Home", new { area = "Admin" });
            //    case 2: // Seller
            //        return RedirectToAction("Index", "Home", new { area = "Seller" });
            //    default: // Người dùng thông thường
            //        return RedirectToAction("Index", "Home");
            //}

            //Nếu có returnUrl hợp lệ, chuyển đến đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            //Ngược lại chuyển đến Index Home
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public IActionResult DangKyTaiKhoan()
        {
            ViewBag.RoleId = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "Người mua hàng" },
                new SelectListItem { Value = "2", Text = "Người bán hàng" }
            }, "Value", "Text");
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-ky.html", Name = "DangKy")]
        public async Task<IActionResult> DangKyTaiKhoan(RegisterViewModel taikhoan)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Tạo tài khoản mới
                    User tk = new User
                    {
                        Username = taikhoan.Username,
                        PhoneNumber = taikhoan.PhoneNumber.Trim().ToLower(),
                        Email = taikhoan.Email.Trim().ToLower(),
                        RoleId = taikhoan.RoleId,
                        Password = (taikhoan.Password).ToMD5(),
                        Active = true,
                        CreateDate = DateTime.Now,
                    };

                    _context.Add(tk);
                    await _context.SaveChangesAsync();

            //        // Thiết lập session và đăng nhập
            //        HttpContext.Session.SetString("UserID", tk.UserId.ToString());
            //        var claims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, tk.Username),
            //    new Claim("UserID", tk.UserId.ToString()),
            //};

            //        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
            //        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            //        await HttpContext.SignInAsync(claimsPrincipal);

                    // Chuyển hướng đến Dashboard hoặc trang chủ
                    return RedirectToAction("Login", "Accounts");
                }
            }
            catch
            {
                return View(taikhoan);
            }
            return View(taikhoan);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidatePhone(string PhoneNumber)
        {
            var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.PhoneNumber.ToLower() == PhoneNumber.ToLower());
            if (khachhang != null)
                return Json($"Số điện thoại {PhoneNumber} đã được sử dụng.");

            return Json(true);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ValidateEmail(string Email)
        {
            var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
            if (khachhang != null)
                return Json($"Email {Email} đã được sử dụng.");

            return Json(true);
        }
        //[HttpGet]
        //[Route("dang-xuat.html", Name = "DangXuat")]
        //public async Task<IActionResult> Logout()
        //{
        //    // Đăng xuất người dùng
        //    await HttpContext.SignOutAsync();

        //    // Xóa tất cả session
        //    HttpContext.Session.Clear();

        //    // Chuyển hướng về trang chủ hoặc trang mà bạn muốn
        //    return RedirectToAction("Index", "Home");
        //}
        [HttpGet]
        [Route("dang-xuat.html", Name = "DangXuat")]
        public async Task<IActionResult> Logout()
        {
            // Đăng xuất khỏi Cookie Authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa Session
            HttpContext.Session.Clear();

            // Chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }

        //// Action đăng xuất với HTTP POST
        //[HttpPost]
        //[Route("dang-xuat.html", Name = "DangXuatPost")]
        //public async Task<IActionResult> LogoutPost()
        //{
        //    // Đăng xuất người dùng
        //    await HttpContext.SignOutAsync();

        //    // Xóa tất cả session
        //    HttpContext.Session.Clear();

        //    // Chuyển hướng về trang chủ hoặc trang mà bạn muốn
        //    return RedirectToAction("Index", "Home");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("dang-xuat.html", Name = "DangXuatPost")]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}

