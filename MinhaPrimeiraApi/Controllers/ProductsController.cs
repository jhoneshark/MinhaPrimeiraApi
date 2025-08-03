using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.DTOs;
using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Models.Pagination;
using MinhaPrimeiraApi.Repository;

namespace MinhaPrimeiraApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductsRepository productsRepository, IMapper mapper)
        {
            _productsRepository = productsRepository;
            _mapper = mapper;
        }

        [HttpGet("products-paged")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductsPaged([FromQuery] ProductsParameters productsParametersarameters)
        {
            var products = _productsRepository.GetProductsPagination(productsParametersarameters);
            
            // Temos duas formas de fazer essa a primeira 
            // var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            // return Ok(productsDto);

            // Essa a segunda forma
            return Ok(_mapper.Map<IEnumerable<ProductDTO>>(products));
        }

        [HttpGet("product-by-category/{id}")]
        public ActionResult<IEnumerable<ProductDTO>> GetProductByCategorie(int id)
        {
            var product = _productsRepository.GetProductByCategorie(id);

            if (product is null)
                return NotFound();
            
            // var destino = _mapper.Map<Destino>(origem);
            var productDTO = _mapper.Map<IEnumerable<ProductDTO>>(product);
            
            
            return Ok(productDTO);
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProductDTO>> Get()
        {
            // como o GetProducts é IQueryable posso fazer consultas como exemplo abaixo
            // var products = _productsRepository.GetProducts().OrderBy(p => p.Name).ToList();
            // var products = _productsRepository.GetProducts().Where(p => p.Price > 15).ToList();
            // var products = _productsRepository.GetProducts().Select(p => p.Name).ToList();
            
            var products = _productsRepository.GetProducts().ToList();
            
            if (products is null)
                return NotFound();
            
            var productDTO = _mapper.Map<IEnumerable<ProductDTO>>(products);
            
            return Ok(productDTO);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public ActionResult<ProductDTO> Get(int id)
        {
            var produto = _productsRepository.GetProduct(id);
            
            if (produto is null)
            {
                return NotFound("Product not found.");
            }

            var productDTO = _mapper.Map<ProductDTO>(produto);
            
            return Ok(productDTO);
        }

        [HttpPost]
        public ActionResult<ProductDTO> Post(ProductDTO productDto)
        {
            if (productDto is null)
            {
                return BadRequest("Produto invalido.");
            }
            
            var product = _mapper.Map<Product>(productDto);

            var productCreated = _productsRepository.CreateProduct(product);
            
            var newProductDTO = _mapper.Map<ProductDTO>(productCreated);
            
            return new CreatedAtRouteResult("GetProductById", new {id = productCreated.ProductId},  newProductDTO);

        }

        [HttpPut("{id:int:min(1)}")]
        public ActionResult<ProductDTO> Put(int id, ProductDTO productDto)
        {
            if (id != productDto.ProductId)
            {
                return BadRequest("Product id invalid");
            }
            var product = _mapper.Map<Product>(productDto);

            var productATT = _productsRepository.UpdateProduct(product);
            
            var newProductDTO = _mapper.Map<ProductDTO>(productATT);
            
            return Ok(newProductDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProductDTO> Delete(int id)
        {
            var product = _productsRepository.GetProduct(id);
            
            if (product is null)
            {
                return NotFound("Product not found.");
            }

            var productDeleted = _productsRepository.DeleteProduct(id);
            
            var productDTO = _mapper.Map<ProductDTO>(productDeleted);
            
            return Ok(productDTO);
        }
    }
}
