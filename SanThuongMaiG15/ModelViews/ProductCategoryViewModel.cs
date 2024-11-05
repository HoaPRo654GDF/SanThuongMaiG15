using Microsoft.AspNetCore.Mvc.Rendering;
using SanThuongMaiG15.Models;
using System.Collections.Generic;

namespace SanThuongMaiG15.ModelViews
{
    public class ProductCategoryViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public SelectList Categories { get; set; }
        public int SelectedCategoryId { get; set; }
        
    }
}
