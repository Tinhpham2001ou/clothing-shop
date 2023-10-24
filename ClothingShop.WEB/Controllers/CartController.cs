using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using ClothingShop.WEB.DTOs.Requests;
using ClothingShop.WEB.DTOs.Responses;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClothingShop.WEB.Controllers
{
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork<ClothingShopContext> _uow;
        private readonly ClothingShopContext _context;

        public CartController(ClothingShopContext context
                , IUnitOfWork<ClothingShopContext> uow)
        {
            _context = context;
            _uow = uow;
        }

        [HttpPost("product/{productId}")]
        public bool AddItemIntoCart(Guid productId)
        {
            try
            {
                List<Cart> carts;
                var cartJson = HttpContext.Session.GetString("Cart");

                if (!string.IsNullOrEmpty(cartJson))
                {
                    carts = JsonConvert.DeserializeObject<List<Cart>>(cartJson);
                }
                else
                {
                    carts = new List<Cart>();
                }

                var colorId = Guid.Parse(Request.Cookies["colorIdSelected"]);
                var sizedId = Guid.Parse(Request.Cookies["sizeIdSelected"]);
                var quantity = Int32.Parse(Request.Cookies["quantity"]);

                var prod = carts.Where(_ => _.ProductId == productId
                                            && _.ColorId == colorId
                                            && _.SizeId == sizedId)
                                .FirstOrDefault();
                if (prod == null)
                {
                    var product = _context.Products.Where(_ => _.Id == productId).FirstOrDefault();

                    carts.Add(new Cart(productId
                                    , product.ProductName
                                    , product.Images.FirstOrDefault().Link
                                    , _context.Colors.Where(_ => _.Id == colorId).FirstOrDefault().Name
                                    , _context.Sizes.Where(_ => _.Id == sizedId).FirstOrDefault().Name
                                    , product.Price
                                    , colorId
                                    , sizedId
                                    , quantity));
                }
                else
                {
                    prod.Quantity += quantity;
                }

                string cartJ = JsonConvert.SerializeObject(carts);

                HttpContext.Session.SetString("Cart", cartJ);

                HttpContext.Response.Cookies.Append("quantity_cart", carts.Count.ToString(), new CookieOptions
                {
                    Path = "/",
                });
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        [HttpGet("my-products")]
        public IActionResult Carts()
        {
            var userId = Guid.Parse(Request.Cookies["user_id"]);
            if (userId != null)
            {
                var acc = _context.Accounts.FirstOrDefault(_ => _.Id == userId);
                ViewBag.UserName = acc.LastName + " " + acc.FirstName;
                ViewBag.PhoneNumber = acc.PhoneNumber;
                ViewBag.Address = acc.Address;
            }

            var carts = getCarts();
            decimal? totalMoney = 0;
            if(carts.Count > 0)
            {
                foreach (var item in carts)
                {
                    totalMoney += item.Quantity * item.Price;
                }
            }
            ViewBag.TotalMoney = totalMoney;

            return View(carts);
        }

        [HttpPost("my-products/remove/{id}")]
        public bool RemoveProductInCart(Guid id)
        {
            try
            {
                var carts = getCarts();

                var prod = carts.FirstOrDefault(_ => _.Id == id);
                carts.Remove(prod);

                string cartJ = JsonConvert.SerializeObject(carts);
                HttpContext.Session.SetString("Cart", cartJ);

                HttpContext.Response.Cookies.Append("quantity_cart", carts.Count.ToString(), new CookieOptions
                {
                    Path = "/",
                });

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private List<Cart> getCarts()
        {
            List<Cart> carts = new List<Cart>();
            var cartJson = HttpContext.Session.GetString("Cart");

            if (!string.IsNullOrEmpty(cartJson))
            {
                carts = JsonConvert.DeserializeObject<List<Cart>>(cartJson);
            }
            return carts;
        }
    }
}
