using System;
using System.Collections.Generic;

#nullable disable

namespace SanThuongMaiG15.Models
{
    public partial class User
    {
        public User()
        {
            Products = new HashSet<Product>();
        }

        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreateDate { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
