using System;
using System.Collections.Generic;

#nullable disable

namespace SanThuongMaiG15.Models
{
    public partial class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int OrderNumber { get; set; }
        public int Quantity { get; set; }
        public decimal TotalMoney { get; set; }
        public DateTime? CreateDate { get; set; }
        public decimal Price { get; set; }
    }
}
