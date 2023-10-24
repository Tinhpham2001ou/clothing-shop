namespace ClothingShop.WEB.DTOs.Requests
{
    public record struct OrderRequest(string Address, bool IsPaied);
    public record struct ChangeStatusOrder(Guid OrderId);
}
