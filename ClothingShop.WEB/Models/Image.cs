namespace ClothingShop.WEB.Models
{
    public partial class Image
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string Link { get; set; }

        public virtual Product Product { get; set; }
    }
}
