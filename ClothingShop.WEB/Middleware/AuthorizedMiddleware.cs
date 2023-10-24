using ClothingShop.WEB.Constants;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ClothingShop.WEB.Middleware
{
    public class AuthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            if (path.Contains("/admin/"))
            {
                if(!isAccess(context, "ADMIN"))
                {
                    context.Response.Redirect("/unauthorized");
                    return;
                }
            }

            if(path.Contains("/my-products") 
                    || path.Contains("/history") 
                    || path.Contains("/order") 
                )
            {
                if(!isAccess(context, "USER"))
                {
                    context.Response.Redirect("/unauthorized");
                    return;
                }
            }

            await _next(context);
        }

        private bool isAccess(HttpContext context, string role)
        {
            var jwtSecurityToken = gẹtJwtSecurityToken(context);
            if (jwtSecurityToken == null)
                return false;

            Claim roleClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == JwtClaimTypes.Role);
            if(roleClaim != null && roleClaim.Value.Equals(role))
            {
                return true;
            }
            return false;
        }

        private JwtSecurityToken gẹtJwtSecurityToken(HttpContext context)
        {
            var token = context.Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
                return null;

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            if (!jwtTokenHandler.CanReadToken(token))
                return null;

            try
            {
                var securityToken = jwtTokenHandler.ReadJwtToken(token);
                return securityToken;
            }
            catch (ArgumentException ex)
            {
                return null;
            }
        }
    }
}
