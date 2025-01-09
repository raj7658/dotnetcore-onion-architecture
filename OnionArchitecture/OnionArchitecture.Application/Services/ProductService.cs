using OnionArchitecture.Application.Interfaces;
using OnionArchitecture.Domain.Domain;
using OnionArchitecture.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;

        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }

        public async Task<Product> GetProductById(int id,CancellationToken cancellationToken)
        {
            return await _repository.GetById(id, cancellationToken);
        }

        public async Task AddProduct(Product product,CancellationToken cancellationToken)
        {
           await _repository.Add(product,cancellationToken);
        }

    }
}
