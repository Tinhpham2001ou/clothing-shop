using ClothingShop.WEB.DTOs.Requests;
using ClothingShop.WEB.Models;
using ClothingShop.WEB.Utils;
using ClothingShop.WEB.Utils.CloudinaryService;
using ClothingShop.WEB.Utils.Email;
using ClothingShop.WEB.Utils.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace ClothingShop.WEB.Controllers  
{
    [Route("authentication")]
    public class AuthenticationController : Controller
    {
        private readonly ClothingShopContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmail _email;
        private readonly IUploadImage _uploadImage;
        private readonly IUnitOfWork<ClothingShopContext> _uow;

        public AuthenticationController(ClothingShopContext context
            , IConfiguration configuration
            , IEmail email
            , IUnitOfWork<ClothingShopContext> uow
            , IUploadImage uploadImage)
        {
            _context = context;
            _configuration = configuration;
            _email = email;
            _uow = uow;
            _uploadImage = uploadImage;
        }

        [HttpGet("login")]
        public IActionResult Login() 
        { 
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.LoginInvalid = true;
                return RedirectToAction("Login", "Authentication");
            }
            else
            {
                var hasPassword = MD5Util.GetMD5(model.Password);

                var acc = _context.Accounts
                            .Where(_ => _.Email.Equals(model.Email) && _.Password.Equals(hasPassword) && _.IsActive == true)
                            .FirstOrDefault();
                if (acc == null)
                {
                    ViewBag.LoginInvalid = true;
                }
                else
                {
                    HttpContext.Response.Cookies.Append("username", acc.LastName + " " + acc.FirstName, new CookieOptions
                    {
                        Path = "/",             
                    });

                    HttpContext.Response.Cookies.Append("user_id", acc.Id.ToString(), new CookieOptions
                    {
                        Path = "/",             
                    });

                    HttpContext.Response.Cookies.Append("role", acc.Role.RoleName, new CookieOptions
                    {
                        Path = "/",             
                    });

                    HttpContext.Response.Cookies.Append("access_token"
                        , JwtUtil.GetToken(_configuration, acc.Id, acc.Email, acc.Role.RoleName, Int32.Parse(_configuration["Jwt:ExpirationAccessToken"]))
                        , new CookieOptions
                            {
                                Path = "/",
                            }
                        );

                    HttpContext.Response.Cookies.Append("fresher_token"
                        , JwtUtil.GetToken(_configuration, acc.Id, acc.Email, acc.Role.RoleName, Int32.Parse(_configuration["Jwt:ExpirationRefreshToken"]))
                        , new CookieOptions
                        {
                            Path = "/",
                        }
                        );

                    if (acc.Role.RoleName.Contains("ADMIN"))
                        return RedirectToAction("GetProducts", "Administrator");
                    return RedirectToAction("Index", "Home");
                }


            }
            return View();
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                if (request.Password != request.ReTypePassword)
                {
                    ViewBag.ResgisterError = "Mật khẩu không khớp!";
                    return View();
                }

                if (request.PhoneNumber.Length != 10)
                {
                    ViewBag.ResgisterError = "Số điện thoại bao gồm 10 chữ số!";
                    return View();
                }

                if (request.FirstName.Length > 50 || request.LastName.Length > 50)
                {
                    ViewBag.ResgisterError = "Tên không hợp lệ!";
                    return View();
                }

                var acc = _context.Accounts.Where(_ => _.Email == request.Email).FirstOrDefault();
                if (acc != null)
                {
                    ViewBag.ResgisterError = "Email đã tồn tại!";
                    return View();
                }

                var acc1 = _context.Accounts.Where(_ => _.PhoneNumber == request.PhoneNumber).FirstOrDefault();
                if (acc1 != null)
                {
                    ViewBag.ResgisterError = "Số điện thoại đã tồn tại!";
                    return View();
                }

                int randomNumber = await sendCodeAsync(request.Email);

                HttpContext.Response.Cookies.Append("code", MD5Util.GetMD5(randomNumber.ToString()), new CookieOptions
                {
                    Path = "/",
                });

                HttpContext.Response.Cookies.Append("email", request.Email, new CookieOptions
                {
                    Path = "/",
                });

                //upate image
                string link = null;
                if (request.ProfilePicture != null)
                    link = _uploadImage.UploadToCloudinary(request.ProfilePicture);

                var account = new Account()
                {
                    Email = request.Email,
                    Password = MD5Util.GetMD5(request.Password),
                    RoleId = _context.Roles.Where(_ => _.RoleName.ToUpper().Equals("USER")).FirstOrDefault().Id,
                    Avatar = link,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Gender = request.Gender,
                    Address = request.Address,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = false
                };

                _context.Accounts.Add(account);
                await _uow.CompleteAsync();


                return RedirectToAction("ConfirmCode", "Authentication");
            }
            catch (Exception ex)
            {
                ViewBag.ResgisterError = "Thông tin không hợp lệ. Vui lòng kiểm tra lại!";
                return View();
            }
        }

        private async Task<int> sendCodeAsync(string email)
        {
            Random random = new Random();

            int randomNumber = random.Next(100000, 1000000);

            await _email.ConfigMailAsync(_configuration);
            _email.SendAsync(email, "Xác nhận tài khoản", randomNumber + _configuration["Email:Content"]);

            return randomNumber;
        }

        [HttpGet("confirm")]
        public IActionResult ConfirmCode()
        {
            return View();
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmCode(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            var codeConfirm = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;

            var code = Request.Cookies["code"];
            var email = Request.Cookies["email"];

            if (MD5Util.GetMD5(codeConfirm).Equals(code))
            {
                var acc = _context.Accounts.Where(_ => _.Email.Equals(email)).FirstOrDefault();
                if (acc != null)
                {
                    acc.IsActive = true;
                    await _uow.CompleteAsync();
                }
                return RedirectToAction("Login", "Authentication");
            }

            ViewBag.InvalidCode = true;
            return View();
        }

        [HttpGet("resend-code")]
        public async Task ResentCode()
        {
            var email = Request.Cookies["email"];

            int randomNumber = await sendCodeAsync(email);

            HttpContext.Response.Cookies.Append("code", MD5Util.GetMD5(randomNumber.ToString()), new CookieOptions
            {
                Path = "/",
            });
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            var cookies = HttpContext.Request.Cookies;

            foreach (var cookie in cookies)
            {
                Response.Cookies.Delete(cookie.Key);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("forgot-password/email")]
        public IActionResult EnterEmail()
        {
            return View();
        }

        [HttpPost("forgot-password/email")]
        public async Task<IActionResult> EnterEmailAsync(string email)
        {
            var acc = _context.Accounts.Where(_ => _.Email.Equals(email)).FirstOrDefault();
            if(acc == null)
            {
                ViewBag.EmailNotExist = true;
                return View();
            }

            HttpContext.Response.Cookies.Append("email", email, new CookieOptions
            {
                Path = "/",
            });

            int randomNumber = await sendCodeAsync(email);

            HttpContext.Response.Cookies.Append("code", MD5Util.GetMD5(randomNumber.ToString()), new CookieOptions
            {
                Path = "/",
            });

            return RedirectToAction("ConfirmAccountWhenForgotPw", "Authentication");
        }

        [HttpGet("confirm-account")]
        public IActionResult ConfirmAccountWhenForgotPw()
        {
            return View();
        }

        [HttpPost("confirm-account")]
        public async Task<IActionResult> ConfirmAccountWhenForgotPw(string digit1, string digit2, string digit3, string digit4, string digit5, string digit6)
        {
            var codeConfirm = digit1 + digit2 + digit3 + digit4 + digit5 + digit6;

            var code = Request.Cookies["code"];
            var email = Request.Cookies["email"];

            if (MD5Util.GetMD5(codeConfirm).Equals(code))
            {
                var acc = _context.Accounts.Where(_ => _.Email.Equals(email)).FirstOrDefault();
                if (acc != null)
                {
                    acc.Password = "111111";
                    await _uow.CompleteAsync();
                }
                return RedirectToAction("ResetPassword", "Authentication");
            }

            ViewBag.InvalidCode = true;
            return View();
        }

        [HttpGet("reset-password")]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost("reset-password")]
        public async Task<bool> ResetPassword([FromBody] PasswordRequest password)
        {
            var email = Request.Cookies["email"];
            var acc = _context.Accounts.Where(_ => _.Email.Equals(email)).FirstOrDefault();
            if(acc!= null && acc.Password == "111111")
            {
                acc.Password = MD5Util.GetMD5(password.Password);
                await _uow.CompleteAsync();
                return true;
            }

            return false;
        }
    }
}
