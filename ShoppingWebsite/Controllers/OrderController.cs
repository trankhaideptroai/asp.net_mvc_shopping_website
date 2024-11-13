using Microsoft.AspNetCore.Mvc;

namespace ShoppingWebsite.Controllers
{
    public class OrderController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
