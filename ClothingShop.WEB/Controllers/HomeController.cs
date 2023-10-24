using ClothingShop.WEB.Constants;
using ClothingShop.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;

namespace ClothingShop.WEB.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClothingShopContext _context;

        public HomeController(ClothingShopContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder, Guid categoryId)
        {
            ViewBag.SortOrder = sortOrder;

            var categories = _context.Categories.ToList();
            ViewBag.Categories = categories;

            var sportShorts = _context.Products
                .Where(_ => _.Category.CategoryName.ToLower().Contains("quần"))
                .ToList();
            ViewBag.SportShorts = sportShorts;

            var products= _context.Products.ToList();

            if (!string.IsNullOrEmpty(sortOrder))
            {
                if (sortOrder.Equals(Sort.Ascending))
                {
                    products = products.OrderBy(_ => _.Price).ToList();
                }
                else if (sortOrder.Equals(Sort.Descending))
                {
                    products = products.OrderByDescending(_ => _.Price).ToList();
                }
            }

            if(categoryId != Guid.Empty)
            {
                products = products.Where(_ => _.CategoryId == categoryId).ToList();
            }

            products = products.Take(8).ToList();

            products.ForEach(_ => 
            {
                if (_.Description.Length > 300)
                    _.Description = _.Description.Substring(0, 300) + "...";
            });

            return View(products);
        }

    }
}