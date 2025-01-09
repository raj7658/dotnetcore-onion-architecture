using OnionArchitecture.Application.Interfaces;
using OnionArchitecture.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly Dictionary<string, RefreshToken> _refreshTokens = new();  
        // Use database or persistent storage in real-world apps

        public RefreshToken GenerateRefreshToken(string userId)
        {
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };

            _refreshTokens[refreshToken.Token] = refreshToken;

            return refreshToken;
        }

        public bool IsValidRefreshToken(string refreshToken, string userId)
        {
            if (_refreshTokens.TryGetValue(refreshToken, out var storedToken))
            {
                return storedToken.UserId == userId && storedToken.ExpirationDate > DateTime.UtcNow;
            }
            return false;
        }

        public void RemoveRefreshToken(string refreshToken)
        {
            _refreshTokens.Remove(refreshToken);
        }
    }
}
