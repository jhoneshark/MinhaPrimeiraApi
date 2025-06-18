using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.Models;

namespace MinhaPrimeiraApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Category>> get()
        {
            try {
                // o take é para paginar
                var categories = _context.Categories.AsNoTracking().Take(100).ToList();
            
                if (categories is null)
                {
                    return NotFound("Categories not found.");
                }
            
                return Ok(categories);
            } catch (Exception e) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao tentar buscar as categorias.");
            }
        }

        [HttpGet("{id:int:min(1)}", Name = "GetCategoryById")]
        public ActionResult<Category> get(int id)
        {
            
            // throw new Exception("Teste");   Criado para testar a middleare de exceptions
            try {
                var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
                if (category is null)
                {
                    return NotFound("Category not found.");
                }
            
                return category;
            } catch (Exception e) {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um erro ao tentar buscar a categoria.");
            }
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Category>> getCategoriesWithProducts()
        {
            return _context.Categories.Include(p => p.Products).ToList();
        }

        [HttpPost]
        public ActionResult post(Category category)
        {
            if (category is null)
            {
                return BadRequest("Category invalid");
            }

            _context.Categories.Add(category);
            _context.SaveChanges();
            
            return new CreatedAtRouteResult("GetCategoryById", new { id = category.CategoryId }, category);
        }

        [HttpDelete("{id:int}")]
        public ActionResult delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category is null)
            {
                return NotFound("Category not found.");
            }
            
            _context.Categories.Remove(category);
            _context.SaveChanges();
            
            return Ok(category);
        }
    }
}
