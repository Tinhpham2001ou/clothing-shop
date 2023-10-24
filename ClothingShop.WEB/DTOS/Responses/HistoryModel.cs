namespace ClothingShop.WEB.DTOs.Responses
{
    public class HistoryModel
    {
        public Guid OrderId { get; set; }
        public List<Cart> Items { get; set; }
        public string Status { get; set; }
        public decimal? TotalMoney { get; set; }
        public string Address { get; set; }

        public HistoryModel(Guid id, decimal? totalMoney, string address, string status) 
        {
            OrderId = id;
            TotalMoney = totalMoney;
            Address = address;
            Status = status;
        }

        public void setItems(List<Cart> items)
        {
            Items = items;
        }
    }
}
