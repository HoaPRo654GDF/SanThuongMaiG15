using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SanThuongMaiG15.Extension;
using SanThuongMaiG15.Helpper;
using SanThuongMaiG15.Models;
using SanThuongMaiG15.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace SanThuongMaiG15.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly EcC2C_2Context _context;
        public INotyfService _notifyService { get; }
        public CheckoutController(EcC2C_2Context context, INotyfService notifyService, ILogger<AccountsController> logger)
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

        // GET: Checkout/Index
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(string returnUrl = null)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            var taikhoanID = User.FindFirstValue("UserID");

            MuaHangVM model = new MuaHangVM();

            if (taikhoanID != null)

            {

                var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
                model.UserId = khachhang.UserId;

                model.Username = khachhang.Username;

                model.Email = khachhang.Email;

                model.PhoneNumber = khachhang.PhoneNumber;

            }


            ViewBag.GioHang = cart;
            return View(model);
        }

        //[HttpPost]
        //[Route("checkout.html", Name = "Checkout")]
        //public IActionResult Index(MuaHangVM muaHang)
        //{
        //    var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
        //    var taikhoanID = User.FindFirstValue("UserID");
        //    MuaHangVM model = new MuaHangVM();

        //    if (taikhoanID != null)

        //    {

        //        var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
        //        model.UserId = khachhang.UserId;

        //        model.Username = khachhang.Username;

        //        model.Email = khachhang.Email;

        //        model.PhoneNumber = khachhang.PhoneNumber;

        //        _context.Update(khachhang);
        //        _context.SaveChanges();
        //    }
        //    try
        //    {
        //        if (ModelState.IsValid)

        //        {

        //            // Khoi tao don hang

        //            Order donhang = new Order();
        //            donhang.BuyerId = model.UserId;

        //            donhang.Address = model.Address;


        //            donhang.OrderDate = DateTime.Now;

        //            donhang.TransactStatusId = 1; // Don hang moi

        //            donhang.Note = Utilities.StripHTML(model.Note);
        //            donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
        //            _context.Add(donhang);

        //            var result = _context.SaveChanges();
        //            if (result > 0)
        //            {
        //                // Lưu thành công
        //                _notifyService.Success($"Đơn hàng {donhang.OrderId} đặt thành công");
        //            }

        //            //Tạo danh sách đơn hàng
        //            foreach (var item in cart)
        //            {
        //                OrderDetail orderDetail = new OrderDetail();

        //                orderDetail.OrderId = donhang.OrderId;

        //                orderDetail.ProductId = item.product.ProductId;
        //                orderDetail.Quantity = item.quantity;

        //                orderDetail.TotalMoney = donhang.TotalMoney;

        //                orderDetail.Price = item.product.Price;

        //                orderDetail.CreateDate = DateTime.Now;

        //                _context.Add(orderDetail);

        //            }

        //            _context.SaveChanges();


        //            HttpContext.Session.Remove("GioHang");



        //            _notifyService.Success("Đơn hàng đặt thành công");


        //            return RedirectToAction("Success");
        //        }
        //    }
        //    catch (Exception ex) {
        //        ViewBag.GioHang = cart;
        //        return View(model);
        //    }
        //    ViewBag.GioHang = cart;
        //    return View(model);
        //}
        [HttpPost]
        [Route("checkout.html", Name = "Checkout")]
        public IActionResult Index(MuaHangVM muaHang)
        {
            var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
            if (cart == null || !cart.Any())
            {
                _notifyService.Error("Giỏ hàng trống");
                return RedirectToAction("Index", "Home");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var taikhoanID = User.FindFirstValue("UserID");
                    if (string.IsNullOrEmpty(taikhoanID))
                    {
                        _notifyService.Error("Vui lòng đăng nhập");
                        return RedirectToAction("Login", "Accounts");
                    }

                    // Tạo đơn hàng với các trường nullable
                    Order donhang = new Order
                    {
                        BuyerId = Convert.ToInt32(taikhoanID),
                        OrderDate = DateTime.Now,
                        TransactStatusId = 1,
                        TotalMoney = Convert.ToDouble(cart.Sum(x => x.TotalMoney)), // Chuyển sang double
                        Note = muaHang.Note ?? string.Empty, // Xử lý null
                        Address = muaHang.Address,
                        PaymentId = muaHang.PaymentID // Thêm PaymentId từ form
                    };

                    _context.Orders.Add(donhang);
                    var result = _context.SaveChanges();
                    Console.WriteLine($"SaveChanges result: {result}");
                    Console.WriteLine($"Created OrderId: {donhang.OrderId}");
                 
                    if (result > 0)
                    {
                        // Tạo chi tiết đơn hàng
                        foreach (var item in cart)
                        {
                            int maxOrderNumber = (int)_context.OrderDetails
                                .Where(od => od.ProductId == item.product.ProductId)
                                .OrderByDescending(od => od.OrderNumber)
                                .Select(od => od.OrderNumber)
                                .FirstOrDefault();

                            OrderDetail orderDetail = new OrderDetail
                            {
                                OrderId = donhang.OrderId,
                                ProductId = item.product.ProductId,
                                OrderNumber = maxOrderNumber + 1, 
                                Quantity = item.quantity,
                                TotalMoney = Convert.ToDouble(item.TotalMoney),
                                CreateDate = DateTime.Now,
                                Price = Convert.ToDouble(item.product.Price)
                            };

                            _context.OrderDetails.Add(orderDetail);
                        }

                        var detailResult = _context.SaveChanges();
                        if (detailResult > 0)
                        {
                            // Xóa giỏ hàng sau khi đặt hàng thành công
                            HttpContext.Session.Remove("GioHang");
                            _notifyService.Success("Đơn hàng đặt thành công");
                            return RedirectToAction("Success");
                        }
                        else
                        {
                            // Nếu lưu chi tiết thất bại, xóa đơn hàng
                            _context.Orders.Remove(donhang);
                            _context.SaveChanges();
                            _notifyService.Error("Lỗi khi lưu chi tiết đơn hàng");
                        }
                    }
                    else
                    {
                        _notifyService.Error("Lỗi khi tạo đơn hàng");
                    }
                }
                else
                {
                    // Log lỗi validation
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }

                // Nếu có lỗi, trả về view với dữ liệu đã nhập
                ViewBag.GioHang = cart;
                return View(muaHang);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                _notifyService.Error($"Lỗi: {ex.Message}");
                ViewBag.GioHang = cart;
                return View(muaHang);
            }
        }

        //[Route("thanhcong.html", Name = "thanhcong")]
        //public IActionResult Success()
        //{
        //    try
        //    {
        //        var taikhoanID = User.FindFirstValue("UserID");
        //        if (string.IsNullOrEmpty(taikhoanID))
        //        {
        //            return RedirectToAction("Login", "Accounts", new { returnUrl = "/.html" });
        //        }
        //        var khachhang = _context.Users.AsNoTracking().SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
        //        var donhang = _context.Orders
        //       .Where(x => x.BuyerId == Convert.ToInt32(taikhoanID))
        //       .OrderByDescending(x => x.OrderDate)
        //       .FirstOrDefault();

        //        MuaHangSuccessVM successVM = new MuaHangSuccessVM();

        //        successVM.Username = khachhang.Username;
        //        successVM.OrderId = donhang.OrderId;

        //        successVM.PhoneNumber = khachhang.PhoneNumber;

        //        successVM.Address = donhang.Address;

        //        return View(successVM);
        //    }
        //    catch
        //    {

        //        return View();
        //    }
        //}
        [Route("thanhcong.html", Name = "thanhcong")]
        public IActionResult Success()
        {
            try
            {
                var taikhoanID = User.FindFirstValue("UserID");
                if (string.IsNullOrEmpty(taikhoanID))
                {
                    return RedirectToAction("Login", "Accounts", new { returnUrl = "/.html" });
                }

                // Debug logging
                Console.WriteLine($"TaikhoanID: {taikhoanID}");

                // Kiểm tra user
                var khachhang = _context.Users.AsNoTracking()
                    .SingleOrDefault(x => x.UserId == Convert.ToInt32(taikhoanID));
                if (khachhang == null)
                {
                    _notifyService.Error("Không tìm thấy thông tin khách hàng");
                    return RedirectToAction("Index", "Home");
                }

         
                var donhang = _context.Orders
                    .Include(x => x.OrderDetails)  
                    .Where(x => x.BuyerId == Convert.ToInt32(taikhoanID))
                    .OrderByDescending(x => x.OrderDate)
                    .FirstOrDefault();

         
                if (donhang == null)
                {
                    Console.WriteLine("Không tìm thấy đơn hàng cho user này");
                    var allOrders = _context.Orders
                        .Where(x => x.BuyerId == Convert.ToInt32(taikhoanID))
                        .ToList();
                    Console.WriteLine($"Tổng số đơn hàng của user: {allOrders.Count}");
                }
                else
                {
                    Console.WriteLine($"Tìm thấy đơn hàng - OrderId: {donhang.OrderId}");
                }

                
                MuaHangSuccessVM successVM = new MuaHangSuccessVM
                {
                    Username = khachhang.Username,
                    PhoneNumber = khachhang.PhoneNumber,
                    OrderId = donhang?.OrderId ?? 0,  
                    Address = donhang?.Address ?? "Không có thông tin",  
                    Note = donhang.Note ?? string.Empty,
                };

                return View(successVM);
            }
            catch (Exception ex)
            {
                // Log lỗi
                Console.WriteLine($"Error in Success action: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                _notifyService.Error("Có lỗi xảy ra khi hiển thị thông tin đơn hàng");
                return RedirectToAction("Index", "Home");
            }
        }


        //[Route("thanhcong.html", Name = "thanhcong")]
        //public IActionResult Success(MuaHangVM muaHang)
        //{

        //    var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");
        //    if (cart == null || !cart.Any())
        //    {
        //        _notifyService.Error("Giỏ hàng trống");
        //        return RedirectToAction("Index", "Home");
        //    }

        //    try
        //    {
        //        var taikhoanID = User.FindFirstValue("UserID");
        //        if (string.IsNullOrEmpty(taikhoanID))
        //        {
        //            _notifyService.Error("Vui lòng đăng nhập");
        //            return RedirectToAction("Login", "Accounts");
        //        }

        //        // Tạo đơn hàng mới
        //        Order donhang = new Order
        //        {
        //            BuyerId = Convert.ToInt32(taikhoanID),
        //            OrderDate = DateTime.Now,
        //            TransactStatusId = 1,
        //            TotalMoney = Convert.ToDouble(cart.Sum(x => x.TotalMoney)),
        //            Note = muaHang.Note ?? string.Empty, 
        //            Address = muaHang.Address,
        //            PaymentId = muaHang.PaymentID,
        //        };

        //        _context.Orders.Add(donhang);
        //        var result = _context.SaveChanges();

        //        if (result > 0)
        //        {
        //            // Tạo chi tiết đơn hàng cho từng sản phẩm trong giỏ
        //            foreach (var item in cart)
        //            {
        //                OrderDetail orderDetail = new OrderDetail
        //                {
        //                    OrderId = donhang.OrderId,
        //                    ProductId = item.product.ProductId,
        //                    Quantity = item.quantity,
        //                    TotalMoney = Convert.ToDouble(item.TotalMoney),
        //                    CreateDate = DateTime.Now,
        //                    Price = Convert.ToDouble(item.product.Price)
        //                };
        //                _context.OrderDetails.Add(orderDetail);
        //            }

        //            _context.SaveChanges();



        //            // Chuẩn bị thông tin cho trang hiển thị đơn hàng thành công
        //            MuaHangSuccessVM successVM = new MuaHangSuccessVM
        //            {
        //                Username = User.FindFirstValue("Username"),
        //                OrderId = donhang.OrderId,
        //                PhoneNumber = User.FindFirstValue("PhoneNumber"),
        //                Address = donhang.Address,
        //                Note = donhang.Note ?? string.Empty,
        //            };

        //            _notifyService.Success("Đơn hàng đặt thành công");
        //            // Xóa giỏ hàng khỏi Session sau khi đặt hàng thành công
        //            HttpContext.Session.Remove("GioHang");
        //            return View(successVM);

        //        }
        //        else
        //        {
        //            _notifyService.Error("Lỗi khi tạo đơn hàng");
        //            return RedirectToAction("Index", "Checkout");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error in Success action: {ex.Message}");
        //        _notifyService.Error("Có lỗi xảy ra khi xử lý đơn hàng");
        //        return RedirectToAction("Index", "Home");
        //    }
        //}

    }
}
