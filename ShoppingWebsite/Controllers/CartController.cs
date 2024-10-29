using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShoppingWebsite.Data;
using ShoppingWebsite.Models;
using System.Collections.Generic;
using System.Linq;

public class CartController : Controller
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var cart = GetCart();
        SetCartItemCount();
        return View(cart);
    }

    [HttpPost]
    [HttpPost]
    public IActionResult AddToCart(int productId)
    {
        var product = _context.Products.Find(productId);
        if (product == null)
        {
            return NotFound();
        }

        // Get the current cart from session or create a new one if it doesn't exist
        var cart = GetCart();
        var cartItem = cart.SingleOrDefault(c => c.ProductId == productId);

        if (cartItem == null)
        {
            // If the product is not in the cart, add it with a quantity of 1
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = 1 // Initial quantity set to 1
            });
        }
        else
        {
            // If the product already exists in the cart, increment the quantity by 1
            cartItem.Quantity += 1;
        }

        // Save the updated cart back to the session
        SaveCart(cart);

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart(); // Get the cart from the session
        var cartItem = cart.SingleOrDefault(c => c.ProductId == productId);

        if (cartItem != null)
        {
            cart.Remove(cartItem); // Remove the entire product from the cart
        }

        SaveCart(cart); // Save the updated cart back to the session
        return Json(new { success = true });
    }

    private List<CartItem> GetCart()
    {
        var cart = HttpContext.Session.GetString("Cart");
        if (string.IsNullOrEmpty(cart))
        {
            return new List<CartItem>();
        }

        return JsonConvert.DeserializeObject<List<CartItem>>(cart);
    }

    private void SaveCart(List<CartItem> cart)
    {
        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }
    private void SetCartItemCount()
    {
        var cart = HttpContext.Session.GetString("Cart");
        var cartItems = string.IsNullOrEmpty(cart)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(cart);

        ViewBag.CartItemCount = cartItems.Sum(c => c.Quantity);
    }
    [HttpPost]
    public IActionResult Checkout()
    {
        // Giả định rằng bạn đã lưu Username trong session sau khi đăng nhập
        var username = HttpContext.Session.GetString("Username");

        if (username == null)
        {
            // Nếu không có thông tin người dùng, yêu cầu đăng nhập
            return RedirectToAction("Login", "Account");
        }

        // Lấy thông tin khách hàng từ cơ sở dữ liệu
        var customer = _context.Customers.SingleOrDefault(c => c.Username == username);
        if (customer == null)
        {
            return NotFound();
        }

        // Điều hướng đến trang checkout và truyền thông tin khách hàng
        return View("Checkout", customer);
    }
    [HttpPost]
    public IActionResult ConfirmOrder(Customer customer)
    {
        if (ModelState.IsValid)
        {
            // Lưu thông tin cập nhật của khách hàng nếu cần
            var existingCustomer = _context.Customers.SingleOrDefault(c => c.Id == customer.Id);
            if (existingCustomer != null)
            {
                existingCustomer.Address = customer.Address; // Cập nhật địa chỉ nếu có
                existingCustomer.Phone = customer.Phone;     // Cập nhật số điện thoại nếu có
                _context.SaveChanges();
            }

            // Lưu đơn hàng (Order) tại đây (nếu có mô hình Orders)
            // Lưu thông tin giỏ hàng hoặc thông tin cần thiết cho đơn hàng

            TempData["SuccessMessage"] = "Đặt hàng thành công!";
            return RedirectToAction("OrderSuccess");
        }
        TempData["ErrorMessage"] = "Có lỗi xảy ra, vui lòng kiểm tra lại thông tin.";
        // Nếu có lỗi trong dữ liệu nhập, hiển thị lại trang checkout với lỗi
        return View("Checkout", customer);
    }

    public IActionResult OrderSuccess()
    {
        return View();
    }
}
