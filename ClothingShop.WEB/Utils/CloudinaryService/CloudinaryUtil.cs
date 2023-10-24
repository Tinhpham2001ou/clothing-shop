using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ClothingShop.WEB.Utils.CloudinaryService
{
    public class CloudinaryUtil : IUploadImage
    {
        private Cloudinary _cloudinary { get; set; }
        public string _cloudName { get; set; }
        public string _apiKey { get; set; }
        public string _apiSecret { get; set; }
        public CloudinaryUtil(IConfiguration Configuration)
        {
            _cloudName = Configuration["Cloudinary:CloudName"];
            _apiKey = Configuration["Cloudinary:ApiKey"];
            _apiSecret = Configuration["Cloudinary:ApiSecret"];
        }
        private void init()
        {
            _cloudinary = new Cloudinary(new Account(_cloudName, _apiKey, _apiSecret));
            _cloudinary.Api.Secure = true;
        }

        public string UploadToCloudinary(IFormFile file)
        {
            init();
            try
            {
                byte[] bytes;
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                string base64 = Convert.ToBase64String(bytes);
                var prefix = @"data:image/png;base64,";
                var imagePath = prefix + base64;

                // create a new ImageUploadParams object and assign the directory name
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(imagePath),
                    Folder = "Clothing Shop"
                };
                var uploadResult = _cloudinary.Upload(@uploadParams);
                return uploadResult.SecureUrl.AbsoluteUri;
            }
            catch (Exception)
            {
                return "up load failed";
            }
        }
    }
}
