namespace ClothingShop.WEB.DTOs.Responses
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal? Price { get; set; }
        public Guid ColorId { get; set; }
        public Guid SizeId { get; set; }
        public int Quantity { get; set; }

        public Cart(Guid productId, string name, string image,string color, string size, decimal? price, Guid colorId, Guid sizeId, int quantity)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            ProductName = name;
            Image = image;
            Color = color;
            Size = size;
            ColorId = colorId;
            SizeId = sizeId;
            Quantity = quantity;
            Price = price;
        }
    }
}
