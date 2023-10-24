using System.ComponentModel.DataAnnotations;

namespace ClothingShop.WEB.DTOs.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email là trường bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là trường bắt buộc.")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mật khẩu là trường bắt buộc.")]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string ReTypePassword { get; set; }

        [Required(ErrorMessage = "Tên là trường bắt buộc.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Họ là trường bắt buộc.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Giới tính là trường bắt buộc.")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Số điện thoại là trường bắt buộc.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ là trường bắt buộc.")]
        public string Address { get; set; }

        public IFormFile? ProfilePicture { get; set; }
    }

}
