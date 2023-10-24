namespace ClothingShop.WEB.Utils.Email
{
    public interface IEmail
    {
        Task ConfigMailAsync(IConfiguration Configuration);
        Task SendAsync(string toEmail, string subject, string content);
    }
}
