using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OnionArchitecture.Application.Interfaces;
using OnionArchitecture.Application.Models;
using OnionArchitecture.Domain.Domain;
using OnionArchitecture.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IRepository<User> _userRepository;
        private readonly IRefreshTokenService _refreshTokenService;
        public AuthService(IConfiguration configuration, 
            IRefreshTokenService refreshTokenService,
            IRepository<User> userRepository)
        {
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
            _userRepository = userRepository;
        }

        public async Task<User> Authenticate(string username, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByName(x => x.Email == username && x.Password == password, cancellationToken);
            if (user == null || user.Password != password)
            {
                return null;
            }

            return user;
        }


        public string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var keyString = jwtSettings["Key"];

            var key = new System.Security.Cryptography.SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes(keyString));

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Email??"")
            };

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(string userId)
        {
            return _refreshTokenService.GenerateRefreshToken(userId);
        }

        public async Task<string> RefreshAccessToken(string refreshToken, string userId,CancellationToken cancellationToken)
        {
            if (_refreshTokenService.IsValidRefreshToken(refreshToken, userId))
            {
                var id = Convert.ToInt32(userId);
                var user =await _userRepository.GetById(id, cancellationToken);
                return GenerateToken(user);
            }
            throw new Exception("Invalid or expired refresh token");
        }

    }
}
