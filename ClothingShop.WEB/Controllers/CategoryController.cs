using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.WEB.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Category()
        {
            return View();
        }
    }
}
