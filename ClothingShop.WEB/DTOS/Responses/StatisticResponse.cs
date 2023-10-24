namespace ClothingShop.WEB.DTOs.Responses
{
	public class StatisticResponse
	{
		public string ProductName { get; set; }
		public decimal? TotalMoney { get; set; }
		public string Trademark { get; set; }

		public StatisticResponse() { }

        public StatisticResponse(string productName, decimal? totalMoney, string trademark)
        {
            ProductName = productName.Substring(0,  productName.Length > 20 ? 20 : productName.Length);
            TotalMoney = totalMoney;
            Trademark = trademark;
        }
    }
}
