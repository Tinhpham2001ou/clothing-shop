namespace ClothingShop.WEB.DTOs.Requests
{
    public record struct ProductRequest(string ProdName, Guid CategoryId, Guid TrademarkId, string Description
                                        , string Price, string OldPrice, int? TotalStockQuantity
                                        , int? QuantitySold, bool IsActive);
}
