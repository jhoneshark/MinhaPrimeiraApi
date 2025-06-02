using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.Models;

namespace MinhaPrimeiraApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> Get()
        {
            var products = _context.Products.AsNoTracking().ToList();
            
            if (products is null)
            {
                return NotFound("Products not found.");
            }
            
            return Ok(products);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public ActionResult<Product> Get(int id)
        {
            var produto = _context.Products.FirstOrDefault(p => p.CategoryId == id);
            if (produto is null)
            {
                return NotFound("Product not found.");
            }
            
            return produto;
        }

        [HttpPost]
        public ActionResult Post(Product product)
        {
            try {
                if (product is null)
                {
                    return BadRequest("Product invalid");
                }
                _context.Products.Add(product);
                _context.SaveChanges();

                return new CreatedAtRouteResult("GetProductById", new { id = product.ProductId}, product);
            } catch (Exception e) {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um erro ao tentar criar um produto");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest("Product id invalid");
            }

            _context.Entry(product).State = EntityState.Modified;
            _context.SaveChanges();
            
            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
            if (product is null)
            {
                return NotFound("Product not found.");
            }
            
            _context.Products.Remove(product);
            _context.SaveChanges();
            
            return Ok(product);
        }
    }
}
