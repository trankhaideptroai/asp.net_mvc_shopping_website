using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingWebsite.Data;
using ShoppingWebsite.Models;
using System.Linq;

namespace ShoppingWebsite.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa


            // Kiểm tra vai trò của người dùng


            // Thiết lập số lượng sản phẩm trong giỏ hàng
            SetCartItemCount();

            // Logic tìm kiếm sản phẩm
            var products = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString) || p.Price.ToString().Contains(searchString));
            }

            return View(products.ToList());
        }




        public IActionResult Details(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);

        }
        private void SetCartItemCount()
        {
            var cart = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrEmpty(cart)
                ? new List<CartItem>()
                : JsonConvert.DeserializeObject<List<CartItem>>(cart);

            ViewBag.CartItemCount = cartItems.Sum(c => c.Quantity);
        }
    }
}
