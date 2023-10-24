namespace ClothingShop.WEB.Utils.CloudinaryService
{
    public interface IUploadImage
    {
        string UploadToCloudinary(IFormFile file);
    }
}
