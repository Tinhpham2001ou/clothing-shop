using ClothingShop.WEB.DTOs.Requests;
using ClothingShop.WEB.DTOs.Responses;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ClothingShop.WEB.Controllers
{
    [Route("order")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork<ClothingShopContext> _uow;
        private readonly ClothingShopContext _context;

        public OrderController(ClothingShopContext context, IUnitOfWork<ClothingShopContext> uow)
        {
            _context = context;
            _uow = uow;
        }

        [HttpPost]
        public async Task<bool> OrderAsync([FromBody] OrderRequest req)
        {
            try
            {
                List<Cart> carts = new List<Cart>();
                var cartJson = HttpContext.Session.GetString("Cart");

                if (!string.IsNullOrEmpty(cartJson))
                {
                    carts = JsonConvert.DeserializeObject<List<Cart>>(cartJson);
                }
                if (carts.Count > 0)
                {
                    decimal? totalMoney = 0;
                    if (carts.Count > 0)
                    {
                        foreach (var item in carts)
                        {
                            totalMoney += item.Quantity * item.Price;
                        }
                    }

                    var orderId = Guid.NewGuid();
                    var order = new Order()
                    {
                        Id = orderId,
                        AccountId = Guid.Parse(Request.Cookies["user_id"]),
                        TotalMoney = totalMoney,
                        Address = req.Address,
                        IsPaied = req.IsPaied,
                        StaticId = _context.Statics.FirstOrDefault(_ => _.Description.ToLower().Equals("đang giao hàng")).Id
                    };
                    _context.Orders.Add(order);
                    await _uow.CompleteAsync();

                    carts.ForEach(async _ =>
                    {
                        _context.OrderDetails.Add(new OrderDetail(_.Id, orderId, _.ProductId, _.SizeId, _.ColorId, _.Quantity));
                        await _uow.CompleteAsync();
                    });

                    HttpContext.Session.Remove("Cart");
                    HttpContext.Response.Cookies.Append("quantity_cart", "0", new CookieOptions
                    {
                        Path = "/",
                    });

                    return true;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
