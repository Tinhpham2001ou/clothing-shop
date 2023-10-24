using System;
using System.Collections.Generic;

#nullable disable

namespace ClothingShop.WEB.Models
{
    public partial class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SizeId { get; set; }
        public Guid ColorId { get; set; }
        public int? Amount { get; set; }

        public OrderDetail() { }

        public OrderDetail(Guid id
            , Guid orderId
            , Guid productId
            , Guid sizeId
            , Guid colorID
            , int? amount)
        {
            Id = id;
            OrderId = orderId;
            ProductId = productId;
            SizeId = sizeId;
            ColorId = colorID;
            Amount = amount;
        }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Size Size { get; set; }
        public virtual Color Color { get; set; }
    }
}
