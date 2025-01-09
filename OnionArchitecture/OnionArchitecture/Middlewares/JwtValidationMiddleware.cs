using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnionArchitecture.Middlewares
{
    public class JwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtValidationMiddleware> _logger;

        public JwtValidationMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<JwtValidationMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                try
                {
                    var jwtSettings = _configuration.GetSection("Jwt");
                    var keyString = jwtSettings["Key"];
                    var key = new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(keyString));

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var principal = ValidateToken(token, key);

                    if (principal != null)
                    {
                        httpContext.User = principal; // Set the principal in HttpContext.User for further use in controllers
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Token validation failed: ", ex);
                }
            }

            await _next(httpContext);
        }

        private ClaimsPrincipal ValidateToken(string token, byte[] key)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"]
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null; // Invalid token
            }
        }
    }
}
