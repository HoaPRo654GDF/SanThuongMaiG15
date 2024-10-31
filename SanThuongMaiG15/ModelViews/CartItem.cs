using SanThuongMaiG15.Models;

namespace SanThuongMaiG15.ModelViews
{
    public class CartItem
    {
        public Product product { get; set; }    
        public int quantity { get; set; }
        public decimal TotalMoney => quantity * (product?.Price ?? 0); 
    }
}
