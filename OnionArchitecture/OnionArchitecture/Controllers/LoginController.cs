using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.Interfaces;
using OnionArchitecture.ViewModels;

namespace OnionArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest,CancellationToken cancellationToken)
        {
            var user =await _authService.Authenticate(loginRequest.Email, loginRequest.Password, cancellationToken);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            var token = _authService.GenerateToken(user);
            return Ok(new { Token = token });
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest refreshTokenRequest,CancellationToken cancellationToken)
        {
            try
            {
                var newAccessToken =await _authService.RefreshAccessToken(refreshTokenRequest.RefreshToken, refreshTokenRequest.UserId,cancellationToken);
                return Ok(new { AccessToken = newAccessToken });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
