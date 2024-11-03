using System.ComponentModel.DataAnnotations;

namespace SanThuongMaiG15.ModelViews
{
    public class MuaHangSuccessVM
    {
        public int BuyerId { get; set; }

        public int OrderId { get; set; }

        public string Username { get; set; }

        //public string Email { get; set; }
     
        public string PhoneNumber { get; set; }


        public string Address { get; set; }

        public int PaymentID { get; set; }

        public string Note { get; set; }
    }
}
