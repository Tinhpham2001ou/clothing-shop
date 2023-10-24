using ClothingShop.WEB.DTOs.Requests;
using ClothingShop.WEB.DTOs.Responses;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils.CloudinaryService;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace ClothingShop.WEB.Controllers
{
    [Route("admin")]
    public class AdministratorController : Controller
    {
        private readonly ClothingShopContext _context;
        private readonly IUnitOfWork<ClothingShopContext> _uow;
        private readonly IUploadImage _uploadImage;

        public AdministratorController(ClothingShopContext context
                                    , IUnitOfWork<ClothingShopContext> uow
                                    , IUploadImage uploadImage)
        {
            _context = context;
            _uow = uow;
            _uploadImage = uploadImage;
        }

        // Product
        [HttpGet("products")]
        public IActionResult GetProducts()
        {
            var prods = _context.Products.ToList();
            prods.ForEach(_ =>
            {
                if (_.Description.Length > 300)
                    _.Description = _.Description.Substring(0, 100) + "...";
            });
            return View(prods);
        }

        [HttpGet("products/create")]
        public IActionResult CreateProduct()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Trademarks = _context.Trademarks.ToList();
            return View();
        }

        [HttpPost("products/create")]
        public async Task<bool> CreateProductAsync([FromBody] ProductRequest req)
        { 
            try
            {
                var id = Guid.NewGuid();
                var product = new Product()
                {
                    Id = id,
                    ProductName = req.ProdName,
                    CategoryId = req.CategoryId,
                    TrademarkId = req.TrademarkId,
                    Price = decimal.Parse(req.Price),
                    Description = req.Description,
                    TotalStockQuantity = 0,
                    QuantitySold = 0,
                    IsActive = true
                };
                if (!string.IsNullOrEmpty(req.OldPrice))
                    product.OldPrice = decimal.Parse(req.OldPrice);

                _context.Products.Add(product);
                await _uow.CompleteAsync();
                HttpContext.Response.Cookies.Append("product_id", id.ToString(), new CookieOptions
                {
                    Path = "/",
                });
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        [HttpGet("products/create/{id}")]
        public IActionResult Create(Guid id)
        {
            ViewBag.Sizes = _context.Sizes.ToList();
            ViewBag.Colors = _context.Colors.ToList();
            return View();
        }

        [HttpPost("products/create/detail")]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            try
            {
                Guid colorId = Guid.Parse(form["color"]);
                Guid sizeId = Guid.Parse(form["size"]);
                int quantity = Int32.Parse(form["quantity"]);
                IFormFile imageFile = form.Files["image"];

                if(quantity < 0)
                {
                    throw new Exception();
                }

                var stock = _context.StockQuantities
                    .Where(_ => _.SizeId == sizeId && _.ColorId == colorId
                        && _.ProductId == Guid.Parse(Request.Cookies["product_id"]))
                    .FirstOrDefault();
                if(stock == null)
                {
                    var stockQuantity = new StockQuantity()
                    {
                        Id = Guid.NewGuid(),
                        StockQuantityTotal = quantity,
                        ColorId = colorId,
                        SizeId = sizeId,
                        ProductId = Guid.Parse(Request.Cookies["product_id"])
                    };

                    _context.StockQuantities.Add(stockQuantity);
                    var pro = _context.Products.Where(_ => _.Id == stockQuantity.ProductId).FirstOrDefault();
                    if (pro != null)
                    {
                        pro.TotalStockQuantity += quantity;

                        if (imageFile != null)
                        {
                            var image = _uploadImage.UploadToCloudinary(imageFile);
                            var i = new Image()
                            {
                                Id = Guid.NewGuid(),
                                ProductId = stockQuantity.ProductId,
                                Link = image
                            };
                            _context.Images.Add(i);
                        }
                    }
                    await _uow.CompleteAsync();
                }
                else
                {
                    throw new Exception();
                }
                ViewBag.Success = true;
                ViewBag.Sizes = _context.Sizes.ToList();
                ViewBag.Colors = _context.Colors.ToList();
                return View();
            }
            catch(Exception e)
            {
                ViewBag.Success = false;

                ViewBag.Sizes = _context.Sizes.ToList();
                ViewBag.Colors = _context.Colors.ToList();
                return View();
            }
        }

        [HttpGet("products/{id}")]
        public IActionResult ProductDetailAdmin(Guid id)
        {
            var pro = _context.Products.Where(_ => _.Id == id).FirstOrDefault();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Trademarks = _context.Trademarks.ToList();
            return View(pro);
        }

        [HttpPost("products/{id}")]
        public async Task<bool> UpdateProductAdminAsync(Guid id, [FromBody] ProductRequest data)
        {
            try
            {
                var prod = _context.Products.Where(_ => _.Id == id).FirstOrDefault();
                if (prod != null)
                {
                    prod.ProductName = data.ProdName;
                    prod.CategoryId = data.CategoryId;
                    prod.TrademarkId = data.TrademarkId;
                    prod.Description = data.Description;
                    prod.Price = decimal.Parse(data.Price);
                    if(!string.IsNullOrEmpty(data.OldPrice))
                        prod.OldPrice = decimal.Parse(data.OldPrice);
                    prod.TotalStockQuantity = data.TotalStockQuantity;
                    prod.QuantitySold = data.QuantitySold;
                    prod.IsActive = data.IsActive;
                    await _uow.CompleteAsync();
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        [HttpDelete("products/{id}")]
        public async Task<bool> DeleteProduct(Guid id)
        {
            var prod = _context.Products.Where(_ => _.Id == id).FirstOrDefault();
            if (prod != null)
            {
                prod.IsActive = false;
                await _uow.CompleteAsync();
                return true;
            }
            return false;
        }

        // statistic
        [HttpGet("statistic")]
        public IActionResult Statistic()
        {
            return View();
        }

        [HttpGet("statistic/data")]
        public List<StatisticResponse> Getdata()
        {
            var results = new List<StatisticResponse>();
            var prods = _context.Products.ToList();

            foreach (var item in prods)
            {
                var s = new StatisticResponse(item.ProductName
                    , item.OrderDetails.Sum(_ => _.Amount * item.Price)
                    , item.Trademark.Name);

                results.Add(s);
            }
            return results;
        }
    }
}
