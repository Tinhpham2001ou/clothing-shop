using System;
using System.Collections.Generic;

#nullable disable

namespace ClothingShop.WEB.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public decimal? TotalMoney { get; set; }
        public string Address { get; set; }
        public bool? IsPaied { get; set; }
        public Guid StaticId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Static Static { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
