using System;
using System.Collections.Generic;

#nullable disable

namespace ClothingShop.WEB.Models
{
    public partial class StockQuantity
    {
        public Guid Id { get; set; }
        public int? StockQuantityTotal { get; set; }
        public Guid SizeId { get; set; }
        public Guid ColorId { get; set; }
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
        public virtual Color Color { get; set; }
    }
}
