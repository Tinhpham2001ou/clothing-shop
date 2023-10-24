using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.WEB.Controllers
{
    public class HelperController : Controller
    {
        [HttpGet("unauthorized")]
        public IActionResult Unauthorized()
        {
            return View();
        }
    }
}
