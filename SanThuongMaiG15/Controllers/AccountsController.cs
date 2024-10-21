//using AspNetCoreHero.ToastNotification.Abstractions;
//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using SanThuongMaiG15.Extension;
//using SanThuongMaiG15.Helpper;
//using SanThuongMaiG15.Models;
//using SanThuongMaiG15.ModelViews;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Runtime.InteropServices;
//using System.Security.Claims;
//using System.Threading.Tasks;


//namespace SanThuongMaiG15.Controllers
//{
//    [Authorize]
//    public class AccountsController : Controller
//    {
//        private readonly EcC2CContext _context;
//        public INotyfService _notifyService { get; }
//        public AccountsController(EcC2CContext context, INotyfService notifyService)
//        {
//            _context = context;
//            _notifyService = notifyService;
//        }
//        [Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
//        public IActionResult Dashboard()
//        {
//            var taikhoanID = HttpContext.Session.GetString("UserID");
//            if (taikhoanID != null)

//            {

//                var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
//                if (khachhang != null)
//                {
//                    return View(khachhang);
//                }
//            }
//            return RedirectToAction("Login");
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        [Route("dang-ky.html", Name = "DangKy")]
//        public IActionResult DangKyTaiKhoan()
//        {
//            ViewBag.RoleId = new SelectList(new List<SelectListItem>
//            {
//                new SelectListItem { Value = "1", Text = "Người mua hàng" },
//                new SelectListItem { Value = "2", Text = "Người bán hàng" }
//            }, "Value", "Text");
//            return View();
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        [Route("dang-ky.html", Name = "DangKy")]
//        public async Task<IActionResult> DangKyTaiKhoan(RegisterViewModel taikhoan)
//        {
//            try
//            {

//                if (ModelState.IsValid)
//                {
//                    User tk = new User
//                    {
//                        Username = taikhoan.Username,
//                        PhoneNumber = taikhoan.PhoneNumber.Trim().ToLower(),
//                        Email = taikhoan.Email.Trim().ToLower(),
//                        RoleId = taikhoan.RoleId,
//                        Password = (taikhoan.Password).ToMD5(),
//                        Active = true,
//                        CreateDate = DateTime.Now,
//                    };
//                    try
//                    {

//                        _context.Add(tk);
//                        await _context.SaveChangesAsync();
//                        HttpContext.Session.SetString("UserID", tk.UserId.ToString());
//                        var taikhoanID = HttpContext.Session.GetString("UserID");
//                        var claims = new List<Claim>();
//                        {
//                            new Claim(ClaimTypes.Name, tk.Username);
//                            new Claim("UserID", tk.UserId.ToString());
//                        };
//                        ClaimsIdentity ClaimsIdentity = new ClaimsIdentity(claims, "login");
//                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(ClaimsIdentity);
//                        await HttpContext.SignInAsync(claimsPrincipal);

//                        return RedirectToAction("Dashboard", "Accounts");

//                    }
//                    catch
//                    {
//                        return RedirectToAction("DangKyTaiKhoan", "Accounts");
//                    }

//                }
//                else
//                {
//                    return View(taikhoan);
//                }
//            }
//            catch
//            {
//                return View(taikhoan);
//            }



//        }




//        [HttpGet]
//        [AllowAnonymous]
//        [Route("dang-nhap.html", Name = "DangNhap")]
//        public IActionResult Login(string returnUrl = null)
//        {
//            var taikhoanID = HttpContext.Session.GetString("UserID");
//            if (taikhoanID != null)

//            {

//                return RedirectToAction("Dashboard", "Accounts");

//            }
//            return View();
//        }


//        [HttpPost]
//        [AllowAnonymous]
//        [Route("dang-nhap.html", Name = "DangNhap")]
//        public async Task<IActionResult> Login(LoginViewModel user, string returnUrl = null)
//        {
//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    bool isEmail = Utilities.IsValidEmail(user.Email);
//                    if (!isEmail) return View(user);
//                    var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.Email.Trim() == user.Email);

//                    if (khachhang != null) return RedirectToAction("DangKyTaiKhoan");
//                    string pass = (user.Password).ToMD5();
//                    if (user.Password != pass)
//                    {
//                        _notifyService.Success("Thông tin đăng nhập sai!");
//                        return View(user);
//                    }
//                    if (khachhang.Active == false) return RedirectToAction("ThongBao", "Accounts");

//                    LUu Session MaKh

//                        HttpContext.Session.SetString("UserID", khachhang.UserId.ToString());
//                    var taikhoanID = HttpContext.Session.GetString("UserID");
//                    Identity

//                    var claims = new List<Claim>
//                    {
//                            new Claim ( ClaimTypes.Email, khachhang.Email) ,

//                            new Claim("UserID", khachhang.UserId.ToString()) ,

//                    };

//                    ClaimsIdentity ClaimsIdentity = new ClaimsIdentity(claims, "Login");
//                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(ClaimsIdentity);
//                    await HttpContext.SignInAsync(claimsPrincipal);
//                    _notifyService.Success("Đăng nhập thành công");
//                    return RedirectToAction("Dashboard", "Accounts");
//                }
//            }
//            catch
//            {
//                return RedirectToAction("DangKyTaiKhoan", "Accounts");
//            }
//            return View(user);
//        }


//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult ValidatePhone(string PhoneNumber)

//        {
//            try

//            {

//                var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.PhoneNumber.ToLower() == PhoneNumber.ToLower());
//                if (khachhang != null)

//                    return Json(data: "Số điện thoai : " + PhoneNumber + " Đã được sử dụng ");
//                return Json(data: true);

//            }

//            catch
//            {
//                return Json(data: true);
//            }
//        }
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult ValidateEmail(string Email)

//        {
//            try

//            {

//                var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.Email.ToLower() == Email.ToLower());
//                if (khachhang != null)

//                    return Json(data: "Email : " + Email + " Đã được sử dụng <br/ >");
//                return Json(data: true);

//            }

//            catch
//            {
//                return Json(data: true);
//            }
//        }


//    }
//}
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

        //[Route("tai-khoan-cua-toi.html", Name = "Dashboard")]
        //[Authorize]
        //public IActionResult Dashboard()
        //{
        //    var taikhoanID = HttpContext.Session.GetString("UserID");
        //    if (taikhoanID != null)
        //    {
        //        var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
        //        if (khachhang != null)
        //        {
        //            return View(khachhang);
        //        }
        //    }
        //    return RedirectToAction("Login");
        //}
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

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        //public IActionResult Login(string returnUrl = null)
        //{
        //    var taikhoanID = HttpContext.Session.GetString("UserID");
        //    if (taikhoanID != null)
        //    {
        //        // Nếu người dùng đã đăng nhập, chuyển hướng đến Dashboard
        //        return RedirectToAction("Dashboard", "Accounts");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        //public async Task<IActionResult> Login(LoginViewModel user, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var khachhang = await _context.Users.AsNoTracking()
        //            .SingleOrDefaultAsync(x => x.Email.Trim() == user.Email.Trim());

        //        if (khachhang == null || khachhang.Password != (user.Password).ToMD5())
        //        {
        //            _notifyService.Success("Thông tin đăng nhập không hợp lệ!");
        //            return View(user);
        //        }

        //        var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Email, khachhang.Email),
        //            new Claim("UserID", khachhang.UserId.ToString())
        //        };

        //        var claimsIdentity = new ClaimsIdentity(claims, "login");
        //        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        //        await HttpContext.SignInAsync(claimsPrincipal);

        //        _notifyService.Success("Đăng nhập thành công!");
        //        return RedirectToAction("Dashboard", "Accounts");
        //    }
        //    return View(user);
        //}
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        //public IActionResult Login(string returnUrl = null)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Dashboard");
        //    }
        //    return View();
        //}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("dang-nhap.html", Name = "DangNhap")]
        //public async Task<IActionResult> Login(LoginViewModel user, string returnUrl = null)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var khachhang = await _context.Users
        //            .AsNoTracking()
        //            .SingleOrDefaultAsync(x => x.Email == user.Email.Trim());

        //        if (khachhang == null || khachhang.Password != user.Password.ToMD5())
        //        {
        //            _notifyService.Error("Thông tin đăng nhập không hợp lệ!");
        //            return View(user);
        //        }

        //        var claims = new List<Claim>
        //    {
        //        new Claim(ClaimTypes.Email, khachhang.Email),
        //        new Claim("UserID", khachhang.UserId.ToString())
        //    };

        //        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

        //        _notifyService.Success("Đăng nhập thành công!");

        //        return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
        //            ? Redirect(returnUrl)
        //            : RedirectToAction("Dashboard");
        //    }
        //    return View(user);
        //}
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
        new Claim("UserID", khachhang.UserId.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

            _notifyService.Success("Đăng nhập thành công!");

            // Nếu có returnUrl hợp lệ, chuyển đến đó
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            // Ngược lại chuyển đến Dashboard
            return RedirectToAction("Dashboard", "Accounts");
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

