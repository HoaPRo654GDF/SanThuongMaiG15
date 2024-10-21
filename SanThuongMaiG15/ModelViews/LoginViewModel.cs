using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace SanThuongMaiG15.ModelViews
{
    public class LoginViewModel
    {
        [MaxLength(100)]

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Địa chỉ Email")]
        public string Email { get; set; }

        [Display(Name = "Mật Khẩu")]

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]

        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 8 kí tự")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
