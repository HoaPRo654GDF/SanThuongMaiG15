using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AccountsController> _logger;
        public AccountsController(EcC2CContext context, INotyfService notifyService, ILogger<AccountsController> logger)
        {
            _context = context;
            _notifyService = notifyService;
            _logger = logger;
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

            //// Kiểm tra nếu người dùng chưa đăng nhập
            //var userId = HttpContext.Session.GetInt32("UserID");
            //if (userId == null)
            //{
            //    return RedirectToAction("Login");
            //}
            //var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == userId);
            //if (khachhang != null)
            //{
            //    return View(khachhang);
            //}

            // Lấy ID người dùng từ claims
            var userIdClaim = User.FindFirst("UserID")?.Value;
            if (int.TryParse(userIdClaim, out int taikhoanID))
            {
                var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == taikhoanID);
                if (khachhang != null)
                {
                    return View(khachhang);
                }
            }


            return RedirectToAction("Login");
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,PhoneNumber")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var existingUser = await _context.Users.FindAsync(id);
                    if (existingUser == null)
                    {
                        return NotFound(); 
                    }

                   
                    existingUser.Username = user.Username;
                    existingUser.Email = user.Email;
                    existingUser.PhoneNumber = user.PhoneNumber;

                  
                    _context.Update(existingUser);
                    await _context.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound(); 
                    }
                    else
                    {
                        throw; 
                    }
                }
                return RedirectToAction("Dashboard"); 
            }

            
            return View(user);
        }

       
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "DangNhap")]
        public IActionResult Login(string returnUrl = "/")
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                
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

            // Lưu thông tin người dùng vào Session
            HttpContext.Session.SetInt32("UserID", khachhang.UserId);
            HttpContext.Session.SetString("UserName", khachhang.Email);
            HttpContext.Session.SetInt32("RoleID", khachhang.RoleId);

            // Tạo Claims và đăng nhập người dùng
            var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, khachhang.Email),
                new Claim("UserID", khachhang.UserId.ToString()),
                new Claim(ClaimTypes.Role, khachhang.RoleId.ToString())
                };


            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            _notifyService.Success("Đăng nhập thành công!");
            _logger.LogInformation($"Email: {khachhang.Email}, UserID: {khachhang.UserId}, RoleID: {khachhang.RoleId}");

            //Nếu có returnUrl hợp lệ, chuyển đến đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

           
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

                    ////Thiết lập session và đăng nhập
                    //HttpContext.Session.SetString("UserID", tk.UserId.ToString());
                    //var claims = new List<Claim>
                    //        {
                    //          new Claim(ClaimTypes.Name, tk.Username),
                    //          new Claim("UserID", tk.UserId.ToString()),
                    //        };

                    //ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    //ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    //await HttpContext.SignInAsync(claimsPrincipal);


                    ////// Thiết lập session đăng nhập
                    ////HttpContext.Session.SetInt32("UserID", tk.UserId);
                    ////HttpContext.Session.SetString("UserName", tk.Email);
                    ////HttpContext.Session.SetInt32("RoleID", tk.RoleId);

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

