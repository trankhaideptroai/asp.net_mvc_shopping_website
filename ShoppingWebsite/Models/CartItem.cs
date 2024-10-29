using Microsoft.AspNetCore.Mvc;

namespace ShoppingWebsite.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; } // Đây là khóa chính
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

    }

}
