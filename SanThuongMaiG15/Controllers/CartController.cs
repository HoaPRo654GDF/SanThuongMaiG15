using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Newtonsoft.Json;
using SanThuongMaiG15.Extension;
using SanThuongMaiG15.Models;
using SanThuongMaiG15.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanThuongMaiG15.Controllers
{
    public class CartController : Controller
    {
        private readonly EcC2C_2Context _context;
        public INotyfService _notifyService { get; }
        private readonly ILogger<CartController> _logger;
        public CartController(EcC2C_2Context context, INotyfService notifyService, ILogger<CartController> logger)
        {
            _context = context;
            _notifyService = notifyService;
            _logger = logger;
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
        //        List<CartItem> gioHang = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();

        //        CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
        //        if (item != null)
        //        {

        //            item.quantity += quantity ?? 1;
        //        }
        //        else
        //        {

        //            Product gh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
        //            item = new CartItem
        //            {
        //                quantity = quantity ?? 1,
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
                // Log chi tiết thông tin đầu vào
                _logger.LogInformation($"Thêm sản phẩm vào giỏ hàng. ProductID: {productID}, Quantity: {quantity}");

                // Kiểm tra thông tin đầu vào
                if (productID <= 0)
                {
                    _logger.LogWarning("Mã sản phẩm không hợp lệ");
                    return Json(new
                    {
                        success = false,
                        message = "Mã sản phẩm không hợp lệ"
                    });
                }

                // Lấy giỏ hàng từ session
                List<CartItem> gioHang = HttpContext.Session.Get<List<CartItem>>("GioHang") ?? new List<CartItem>();

                // Tìm sản phẩm trong giỏ hàng
                CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);

                if (item != null)
                {
                    // Nếu sản phẩm đã có trong giỏ, tăng số lượng
                    item.quantity += quantity ?? 1;
                    _logger.LogInformation($"Cập nhật số lượng sản phẩm. Mã SP: {productID}, Số lượng mới: {item.quantity}");
                }
                else
                {
                    // Tìm sản phẩm trong database
                    Product gh = _context.Products.SingleOrDefault(p => p.ProductId == productID);

                    if (gh == null)
                    {
                        _logger.LogWarning($"Không tìm thấy sản phẩm có ID: {productID}");
                        return Json(new
                        {
                            success = false,
                            message = "Sản phẩm không tồn tại"
                        });
                    }

                    // Tạo mục mới trong giỏ hàng
                    item = new CartItem
                    {
                        quantity = quantity ?? 1,
                        product = gh
                    };
                    gioHang.Add(item);
                    _logger.LogInformation($"Thêm sản phẩm mới vào giỏ hàng. Mã SP: {productID}, Số lượng: {item.quantity}");
                }

                // Lưu giỏ hàng vào session
                HttpContext.Session.Set<List<CartItem>>("GioHang", gioHang);
                

                return Json(new
                {
                    success = true,
                    message = "Thêm sản phẩm vào giỏ hàng thành công",
                    cartItemCount = gioHang.Count
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                _logger.LogError(ex, $"Lỗi khi thêm sản phẩm vào giỏ hàng. ProductID: {productID}");

                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng"
                });
            }
        }

        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? quantity)
        {
            //lấy giot hàng ra xử lý
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
        //[Route("api/cart/remove")]
        //public ActionResult Remove(int productID)
        //{
        //    try 
        //    {
        //        List<CartItem> gioHang = GioHang;
        //        CartItem item = gioHang.SingleOrDefault(p => p.product.ProductId == productID);
        //        if (item != null)
        //        {
        //            gioHang.Remove(item);
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
        public IActionResult RemoveFromCart([FromBody] RemoveCartItemRequest request)
        {
            try
            {
                // Log chi tiết quá trình
                _logger.LogInformation($"Bắt đầu xóa sản phẩm. ID: {request.ProductId}");

                // Kiểm tra session
                if (!HttpContext.Session.Keys.Contains("GioHang"))
                {
                    _logger.LogWarning("Khóa 'Cart' không tồn tại trong session");
                    return Json(new
                    {
                        success = false,
                        message = "Phiên làm việc đã hết hạn. Vui lòng tải lại trang.",
                        errorCode = "SESSION_EXPIRED"
                    });
                }

                // Lấy giỏ hàng từ session
                var cart = HttpContext.Session.Get<List<CartItem>>("GioHang");

                // Log trạng thái giỏ hàng
                if (cart == null)
                {
                    _logger.LogWarning("Giỏ hàng null sau khi lấy từ session");
                    return Json(new
                    {
                        success = false,
                        message = "Giỏ hàng không tồn tại.",
                        errorCode = "CART_NULL"
                    });
                }

                // Log chi tiết giỏ hàng
                _logger.LogInformation($"Số lượng sản phẩm trong giỏ: {cart.Count}");
                foreach (var item in cart)
                {
                    _logger.LogInformation($"Sản phẩm trong giỏ: ID = {item.product.ProductId}, Tên = {item.product.ProductName}");
                }

                // Tìm và xóa sản phẩm
                var itemToRemove = cart.FirstOrDefault(x => x.product.ProductId == request.ProductId);

                if (itemToRemove == null)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Sản phẩm không tồn tại trong giỏ hàng.",
                        errorCode = "PRODUCT_NOT_FOUND",
                        cartItemCount = cart.Count
                    });
                }

                // Xóa sản phẩm
                cart.Remove(itemToRemove);

                // Lưu lại giỏ hàng
                HttpContext.Session.Set("GioHang", cart);

                _logger.LogInformation($"Đã xóa sản phẩm ID: {request.ProductId}. Số lượng sản phẩm còn lại: {cart.Count}");

                return Json(new
                {
                    success = true,
                    message = "Đã xóa sản phẩm khỏi giỏ hàng.",
                    remainingItemCount = cart.Count
                });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết
                _logger.LogError(ex, $"Lỗi khi xóa sản phẩm: {ex.Message}");

                return Json(new
                {
                    success = false,
                    message = "Có lỗi xảy ra. Vui lòng thử lại.",
                    errorCode = "UNEXPECTED_ERROR",
                    errorDetails = ex.Message
                });
            }
        }


       
        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
        {
            var cart = GioHang;
            var cartItem = cart.FirstOrDefault(c => c.product.ProductId == request.ProductId);
            if (cartItem != null)
            {
                // Cập nhật số lượng sản phẩm
                cartItem.quantity = request.Quantity;

                // Lưu lại giỏ hàng đã cập nhật vào session
                HttpContext.Session.Set("GioHang", cart);

                return Json(new { success = true, message = "Cập nhật số lượng thành công" });
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
        }
    }
}


