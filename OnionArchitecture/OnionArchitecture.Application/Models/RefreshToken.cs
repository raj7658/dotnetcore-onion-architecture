using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
