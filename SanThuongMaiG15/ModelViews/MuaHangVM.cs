using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SanThuongMaiG15.ModelViews
{
    public class MuaHangVM
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Vui long nhập họ tên")]
        public string Username { get; set; }

        public string Email { get; set; }
        [Required(ErrorMessage = "Vui long nhập số điện thoại")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Địa chỉ nhận hàng")]

        public string Address { get; set; }

        public int PaymentID { get; set; }

        public string Note { get; set; }
    }
}
