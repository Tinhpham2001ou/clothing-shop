using System;
using System.Collections.Generic;

#nullable disable

namespace ClothingShop.WEB.Models
{
    public partial class Product
    {
        public Product()
        {
            Images = new HashSet<Image>();
            OrderDetails = new HashSet<OrderDetail>();
            StockQuantities = new HashSet<StockQuantity>();
        }

        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public Guid CategoryId { get; set; }
        public Guid TrademarkId { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public int? TotalStockQuantity { get; set; }
        public int? QuantitySold { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? IsActive { get; set; }

        public virtual Category Category { get; set; }
        public virtual Trademark Trademark { get; set; }
        public virtual ICollection<Image> Images { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<StockQuantity> StockQuantities { get; set; }
    }
}
