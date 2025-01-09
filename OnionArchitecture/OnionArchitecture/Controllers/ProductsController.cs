using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.Interfaces;
using OnionArchitecture.Application.Services;
using OnionArchitecture.Domain.Domain;

namespace OnionArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("/products/{id}")]
        public async Task<IActionResult> GetProductById(int id, CancellationToken cancellationToken)
        {
            var product =await _productService.GetProductById(id, cancellationToken);
            return product != null ? Ok(product) : NotFound();
        }

        [HttpPost("/products")]
        public async Task<IActionResult> AddProduct([FromBody] Product product, CancellationToken cancellationToken)
        {
            await _productService.AddProduct(product, cancellationToken);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }
    }
}
