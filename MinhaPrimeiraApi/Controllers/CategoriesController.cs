using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Caching.Memory;
using MinhaPrimeiraApi.Domain.DTOs;
using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Models.Pagination;
using MinhaPrimeiraApi.Domain.Interface;

namespace MinhaPrimeiraApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private const string CacheCategoryAll = "categories_all";

        public CategoriesController(IUnitOfWork uof, ILogger<CategoriesController> logger, IMapper mapper, IMemoryCache cache)
        {
            _uof = uof;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;
        }

        [HttpGet("categories-paged")]
        public async Task<ActionResult<IEnumerable<CategoryDTO>>> GetCategoriesPaged([FromQuery] CategoriesParameters  categoriesParametersarameters)
        {
            var categories = await _uof.CategoryRepository.GetCategoriesPagination(categoriesParametersarameters);

            var metaData = new
            {
                categories.Count,
                categories.PageSize,
                categories.PageCount,
                categories.TotalItemCount,
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

        // 2. Usa a política "ParceiroVip"
        // Apenas "https://api.parceiro-vip.com" acessa aqui
        // O Default é IGNORADO para este método.
        // [EnableCors("ParceiroVip")] ou [EnableCors("ApenasLeituraPublica")]
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
        
        //essa policy tem uma regra de 3 request a cada 30s
        //[Authorize]
        [HttpGet]
        //[EnableRateLimiting("API_Free")]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            var categories = await _cache.GetOrCreateAsync(
                CacheCategoryAll,
                async entry =>
                {
                    //AbsoluteExpiration é Horário fixo
                    //Tempo fixo desde criação
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    //Tempo sem acesso
                    entry.SlidingExpiration = TimeSpan.FromMinutes(5);
                    entry.Priority = CacheItemPriority.High;
                    return await _uof.CategoryRepository.GetCategories();
                });

            return Ok(categories);
        }

        [HttpGet("{id:int:min(1)}", Name = "GetCategoryById")]
        public async Task<ActionResult<Category>> get(int id)
        {
            var CacheCategoryId = GetCategoryCacheKey(id);
            // exemplo para criar chave para um unico user, apenas exemplo
            //var userId = User.Identity.Name;
            // throw new Exception("Teste");   Criado para testar a middleare de exceptions
            
            _logger.LogInformation($" ************** GET GetCategoryById = {id} **********************");

            var category = await _cache.GetOrCreateAsync(
                CacheCategoryId,
                async entry =>
                {
                    //AbsoluteExpiration é Horário fixo
                    //Tempo fixo desde criação
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                    //Tempo sem acesso
                    entry.SlidingExpiration = TimeSpan.FromMinutes(5);

                    return Task.FromResult(_uof.CategoryRepository.GetCategory(id));
                });
            
            if (category == null)
                return NotFound();
            
            return Ok(category);    
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Category>> getCategoriesWithProducts()
        {
            var categories = _uof.CategoryRepository.GetCategoriesWithProducts();
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(Policy = "RootOnly")]
        public async Task<ActionResult> post(Category category)
        {
            if (category is null)
            {
                return BadRequest("Category invalid");
            }

            var categoryCreate = await _uof.CategoryRepository.CreateCategory(category);
            await _uof.Commit();
            
            _cache.Remove(CacheCategoryAll);
            
            return new CreatedAtRouteResult("GetCategoryById", new { id = categoryCreate.CategoryId }, categoryCreate);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest("Dados invalidos");
            }

            var categoryATT = _uof.CategoryRepository.UpdateCategory(category);
            _uof.Commit();
            
            InvalidateCacheAfterChange(id);
            
            return Ok(categoryATT);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "ExclusivePolicyOnly")]
        public async Task<ActionResult> delete(int id)
        {
            var category = _uof.CategoryRepository.GetCategory(id);
            if (category is null)
            {
                return NotFound("Category not found.");
            }

            var CacheCategoryId = GetCategoryCacheKey(id);

            var categoryDelete =  _uof.CategoryRepository.DeleteCategory(id);
            _uof.Commit();

            InvalidateCacheAfterChange(id);
            
            return Ok(categoryDelete);
        }
        
        private string GetCategoryCacheKey(int id) => $"Category_{id}";
        
        private void InvalidateCacheAfterChange(int id)
        {
            _cache.Remove(CacheCategoryAll);
            _cache.Remove(GetCategoryCacheKey(id));
        }
    }
}
