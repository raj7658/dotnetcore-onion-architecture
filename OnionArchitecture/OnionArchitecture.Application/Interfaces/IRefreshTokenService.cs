using OnionArchitecture.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        RefreshToken GenerateRefreshToken(string userId);
        bool IsValidRefreshToken(string refreshToken, string userId);
        void RemoveRefreshToken(string refreshToken);
    }
}
