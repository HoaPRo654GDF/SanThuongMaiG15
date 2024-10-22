using System;
using System.Collections.Generic;

#nullable disable

namespace SanThuongMaiG15.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int? BuyerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int TransactStatusId { get; set; }
        public decimal TotalMoney { get; set; }
        public string Note { get; set; }
        public string Address { get; set; }

        public virtual User Buyer { get; set; }
        public virtual TransactStatus TransactStatus { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
