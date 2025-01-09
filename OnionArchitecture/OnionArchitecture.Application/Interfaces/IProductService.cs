using OnionArchitecture.Domain.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductById(int id,CancellationToken cancellationToken);
        Task AddProduct(Product product, CancellationToken cancellationToken);
    }
}
