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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;
        private readonly ILogger _logger;
        public CategoriesController(ICategoryRepository repository, ILogger<CategoriesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> get()
        {
            var categories = _repository.GetCategories();
            return Ok(categories);
        }

        [HttpGet("{id:int:min(1)}", Name = "GetCategoryById")]
        public ActionResult<Category> get(int id)
        {
            // throw new Exception("Teste");   Criado para testar a middleare de exceptions
            
            _logger.LogInformation($" ************** GET GetCategoryById = {id} **********************");
            
            var category = _repository.GetCategory(id);

            if (category is null)
            {
                return NotFound();
            }
            
            return Ok(category);    
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Category>> getCategoriesWithProducts()
        {
            var categories = _repository.GetCategoriesWithProducts();
            return Ok(categories);
        }

        [HttpPost]
        public ActionResult post(Category category)
        {
            if (category is null)
            {
                return BadRequest("Category invalid");
            }

            var categoryCreate = _repository.CreateCategory(category);
            
            return new CreatedAtRouteResult("GetCategoryById", new { id = categoryCreate.CategoryId }, categoryCreate);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Dados invalidos");
            }

            var categoryATT = _repository.UpdateCategory(category);
            return Ok(categoryATT);
        }

        [HttpDelete("{id:int}")]
        public ActionResult delete(int id)
        {
            var category = _repository.GetCategory(id);
            if (category is null)
            {
                return NotFound("Category not found.");
            }

            var categoryDelete = _repository.DeleteCategory(id);
            
            return Ok(categoryDelete);
        }
    }
}
