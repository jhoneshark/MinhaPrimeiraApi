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
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriesController(IUnitOfWork uof, ILogger<CategoriesController> logger, IMapper mapper)
        {
            _uof = uof;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("categories-paged")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesPaged([FromQuery] CategoriesParameters  categoriesParametersarameters)
        {
            var categories = await _uof.CategoryRepository.GetCategoriesPagination(categoriesParametersarameters);

            var metaData = new
            {
                categories.TotalCount,
                categories.PageSize,
                categories.CurrentPage,
                categories.TotalPages,
                categories.HasNextPage,
                categories.HasPreviousPage,
            };
            
            var categoriesDto = _mapper.Map<IEnumerable<CategoryDTO>>(categories);

            var response = new
            {
                categories = categoriesDto,
                paginete = metaData
            };
            
            return Ok(response);
        }

        [HttpGet("filter/name/paged")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesFilteredName([FromQuery] CategoriesFilterName categoriesParametersarameters)
        {
            var categories = await _uof.CategoryRepository.GetCategoriesFilterName(categoriesParametersarameters);

            return CategoriesFilteredName(categories);
        }
        
        private ActionResult<IEnumerable<CategoryDTO>> CategoriesFilteredName(PagedList<Category> category)
        {
            var metaData = new
            {
                category.TotalCount,
                category.PageSize,
                category.CurrentPage,
                category.TotalPages,
                category.HasNextPage,
                category.HasPreviousPage,
            };
            
            var productDto = _mapper.Map<IEnumerable<CategoryDTO>>(category);
                
            var response = new
            {
                categories = productDto,
                paginete = metaData
            };
            
            return Ok(response);
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<Category>> get()
        {
            var categories = _uof.CategoryRepository.GetCategories();
            return Ok(categories);
        }

        [HttpGet("{id:int:min(1)}", Name = "GetCategoryById")]
        public ActionResult<Category> get(int id)
        {
            // throw new Exception("Teste");   Criado para testar a middleare de exceptions
            
            _logger.LogInformation($" ************** GET GetCategoryById = {id} **********************");
            
            var category = _uof.CategoryRepository.GetCategory(id);

            if (category is null)
            {
                return NotFound();
            }
            
            return Ok(category);    
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Category>> getCategoriesWithProducts()
        {
            var categories = _uof.CategoryRepository.GetCategoriesWithProducts();
            return Ok(categories);
        }

        [HttpPost]
        public ActionResult post(Category category)
        {
            if (category is null)
            {
                return BadRequest("Category invalid");
            }

            var categoryCreate = _uof.CategoryRepository.CreateCategory(category);
            _uof.Commit();
            
            return new CreatedAtRouteResult("GetCategoryById", new { id = categoryCreate.CategoryId }, categoryCreate);
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Dados invalidos");
            }

            var categoryATT = _uof.CategoryRepository.UpdateCategory(category);
            _uof.Commit();
            
            return Ok(categoryATT);
        }

        [HttpDelete("{id:int}")]
        public ActionResult delete(int id)
        {
            var category = _uof.CategoryRepository.GetCategory(id);
            if (category is null)
            {
                return NotFound("Category not found.");
            }

            var categoryDelete = _uof.CategoryRepository.DeleteCategory(id);
            _uof.Commit();
            
            return Ok(categoryDelete);
        }
    }
}
