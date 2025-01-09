using OnionArchitecture.Application.Models;
using OnionArchitecture.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Interfaces
{
    public interface IAuthService
    {
        Task<User> Authenticate(string username, string password,CancellationToken cancellationToken);
        string GenerateToken(User user);
        RefreshToken GenerateRefreshToken(string userId);
        Task<string> RefreshAccessToken(string refreshToken, string userId,CancellationToken cancellationToken);
    }
}
