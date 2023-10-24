using ClothingShop.WEB.Constants;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ClothingShop.WEB.Controllers
{
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork<ClothingShopContext> _uow;
        private readonly ClothingShopContext _context;
        private static Guid _productId;

        public ProductController(IUnitOfWork<ClothingShopContext> uow
            , ClothingShopContext context)
        {
            _uow = uow;
            _context = context;
        }

        [HttpGet("list")]
        public IActionResult Products(int page = 1, string search = "", string sort = "")
        {
            ViewBag.Search = search;

            var products = _context.Products.Where(_ => _.IsActive == true 
                                                        && (string.IsNullOrWhiteSpace(search) || _.ProductName.Contains(search)))
                                            .ToList();
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals(Sort.Ascending))
                {

                    ViewBag.Sort = Sort.Ascending;
                    products = products.OrderBy(_ => _.Price).ToList();
                }
                else if (sort.Equals(Sort.Descending))
                {
                    ViewBag.Sort = Sort.Descending;
                    products = products.OrderByDescending(_ => _.Price).ToList();
                }
            }
            else
                ViewBag.Sort = Sort.Default;

            ViewBag.PageCount = (int)Math.Ceiling((double)products.Count() / Paging.SizePage);

            var listProd = products.Skip((page - 1) * Paging.SizePage).Take(Paging.SizePage).ToList();
            listProd.ForEach(_ =>
            {
                if (_.Description.Length > 300)
                    _.Description = _.Description.Substring(0, 300) + "...";
            });

            return View(listProd);
        }


        [HttpGet("detail/{id}")]
        public IActionResult ProductDetails(Guid id)
        {
            if (id != _productId) 
            { 
                removeCookies();
                _productId = id;
            }

            var product = _context.Products.Where(_ => _.Id == id).FirstOrDefault();

            var stockQuantities = product.StockQuantities;
            ViewBag.StockQuantities = stockQuantities;

            if (product == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ColorIdSelected = Request.Cookies["colorIdSelected"];
            ViewBag.SizeIdSelected = Request.Cookies["sizeIdSelected"];
            ViewBag.Quantity = Request.Cookies["quantity"];

            return View(product);
        }

        private void removeCookies()
        {
            HttpContext.Response.Cookies.Append("quantity", "1", new CookieOptions
            {
                Path = "/",
            });
            HttpContext.Response.Cookies.Append("colorIdSelected", "", new CookieOptions
            {
                Path = "/",
            });
            HttpContext.Response.Cookies.Append("sizeIdSelected", "", new CookieOptions
            {
                Path = "/",
            });
        }
    }
}
