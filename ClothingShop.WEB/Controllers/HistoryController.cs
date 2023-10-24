using ClothingShop.WEB.DTOs.Requests;
using ClothingShop.WEB.DTOs.Responses;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Reflection.Metadata;

namespace ClothingShop.WEB.Controllers
{
    [Route("history")]
    public class HistoryController : Controller
    {
        private readonly ClothingShopContext _context;
        private readonly IUnitOfWork<ClothingShopContext> _uow;
        public HistoryController(ClothingShopContext context
            , IUnitOfWork<ClothingShopContext> uow)
        {
            _context = context;
            _uow = uow;
        }

        [HttpGet]
        public IActionResult History()
        {
            try
            {
                var userId = Request.Cookies["user_id"];
                var acc = _context.Accounts
                    .Where(_ => !string.IsNullOrEmpty(userId) && _.Id == Guid.Parse(userId))
                    .FirstOrDefault();
                if (acc == null)
                    return RedirectToAction("Index", "Home");

                // shiping
                var orders = _context.Orders
                    .Where(_ => _.AccountId == acc.Id
                            && _.Static.Description.ToLower().Contains("đang giao hàng"))
                    .ToList();

                var status = "Đang giao hàng";

                var results = new List<HistoryModel>();
                orders.ForEach(o =>
                {
                    var historyModel = new HistoryModel(o.Id, o.TotalMoney, o.Address, status);

                    var items = new List<Cart>();
                    foreach (var i in o.OrderDetails)
                    {
                        var item = new Cart(i.ProductId, i.Product.ProductName
                            , i.Product.Images.FirstOrDefault().Link
                            , _context.Colors.FirstOrDefault(_ => _.Id == i.ColorId).Name
                            , _context.Sizes.FirstOrDefault(_ => _.Id == i.SizeId).Name
                            , _context.Products.FirstOrDefault(_ => _.Id == i.ProductId).Price * i.Amount
                            , i.ColorId
                            , i.SizeId
                            , (int)i.Amount);
                        items.Add(item);
                    }

                    historyModel.setItems(items);

                    results.Add(historyModel);
                });

                //received 
                var ordersReceived = _context.Orders
                    .Where(_ => _.AccountId == acc.Id
                            && _.Static.Description.ToLower().Contains("đã nhận hàng"))
                    .ToList();

                var statusReceived = "Đã nhận hàng";

                var resultsReceived = new List<HistoryModel>();
                ordersReceived.ForEach(o =>
                {
                    var historyModel = new HistoryModel(o.Id, o.TotalMoney, o.Address, statusReceived);

                    var items = new List<Cart>();
                    foreach (var i in o.OrderDetails)
                    {
                        var item = new Cart(i.ProductId, i.Product.ProductName
                            , i.Product.Images.FirstOrDefault().Link
                            , _context.Colors.FirstOrDefault(_ => _.Id == i.ColorId).Name
                            , _context.Sizes.FirstOrDefault(_ => _.Id == i.SizeId).Name
                            , _context.Products.FirstOrDefault(_ => _.Id == i.ProductId).Price * i.Amount
                            , i.ColorId
                            , i.SizeId
                            , (int)i.Amount);
                        items.Add(item);
                    }

                    historyModel.setItems(items);

                    resultsReceived.Add(historyModel);
                });
                ViewBag.ItemsReceived = resultsReceived;

                return View(results);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("status/true")]
        public async Task<bool> UpdateStatus([FromBody] ChangeStatusOrder req)
        {
            var order = _context.Orders.Where(_ => _.Id == req.OrderId).FirstOrDefault();
            if (order == null)
                return false;

            order.StaticId = _context.Statics.FirstOrDefault(_ => _.Description.ToLower().Contains("đã nhận hàng")).Id;
            await _uow.CompleteAsync();
            return true;
        }
    }
}
