using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Repository;

namespace MinhaPrimeiraApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _productsRepository;

        public ProductsController(IProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            // como o GetProducts é IQueryable posso fazer consultas como exemplo abaixo
            // var products = _productsRepository.GetProducts().OrderBy(p => p.Name).ToList();
            // var products = _productsRepository.GetProducts().Where(p => p.Price > 15).ToList();
            // var products = _productsRepository.GetProducts().Select(p => p.Name).ToList();
            
            var products = _productsRepository.GetProducts().ToList();
            
            return Ok(products);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public ActionResult<Product> Get(int id)
        {
            var produto = _productsRepository.GetProduct(id);
            
            if (produto is null)
            {
                return NotFound("Product not found.");
            }
            
            return Ok(produto);
        }

        [HttpPost]
        public ActionResult Post(Product product)
        {
            if (product is null)
            {
                return BadRequest("Produto invalido.");
            }

            var productCreated = _productsRepository.CreateProduct(product);
            
            return new CreatedAtRouteResult("GetProductById", new {id = productCreated.ProductId},  productCreated);

        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult Put(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Product id invalid");
            }

            var productATT = _productsRepository.UpdateProduct(product);
            return Ok(productATT);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var product = _productsRepository.GetProduct(id);
            
            if (product is null)
            {
                return NotFound("Product not found.");
            }

            var productDeleted = _productsRepository.DeleteProduct(id);
            return Ok(productDeleted);
        }
    }
}
