using System;
using System.Collections.Generic;

#nullable disable

namespace SanThuongMaiG15.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int CatId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int? SellerId { get; set; }
        public DateTime? DatePosted { get; set; }
        public string ImageUrl { get; set; }
        public string ProductStatus { get; set; }

        public virtual Category Cat { get; set; }
        public virtual User Seller { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
