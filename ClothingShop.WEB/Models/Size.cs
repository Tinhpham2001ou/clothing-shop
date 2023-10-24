using System;
using System.Collections.Generic;

#nullable disable

namespace ClothingShop.WEB.Models
{
    public partial class Size
    {
        public Size()
        {
            StockQuantities = new HashSet<StockQuantity>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<StockQuantity> StockQuantities { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
