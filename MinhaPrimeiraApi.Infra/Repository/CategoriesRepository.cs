using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Infra.Context;
using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Models.Pagination;
using MinhaPrimeiraApi.Domain.Interface;
using X.PagedList;
using X.PagedList.EF;

namespace MinhaPrimeiraApi.Domain.Repository;

public class CategoriesRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoriesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IPagedList<Category>> GetCategoriesPagination(CategoriesParameters categoriesParameters)
    {
        var categoriesQuery = _context.Categories
            .AsNoTracking()
            .OrderBy(p => p.CategoryId)
            .AsQueryable();
        
        var result = await categoriesQuery.ToPagedListAsync(categoriesParameters.PageNumber, categoriesParameters.PageSize);
        
        return result;
    }

    public async Task<Models.Pagination.PagedList<Category>> GetCategoriesFilterName(CategoriesFilterName categoriesParameters)
    {
        var categories = _context.Categories
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .AsQueryable();

        if (!string.IsNullOrEmpty(categoriesParameters.Name))
        {
            categories = categories.Where(p => p.Name.ToLower().Contains(categoriesParameters.Name.ToLower()));
        }

        var categoriesFiltered = await Models.Pagination.PagedList<Category>.ToPagedList(categories, categoriesParameters.PageNumber, categoriesParameters.PageSize);
        
        return categoriesFiltered;
    }

    public async Task<IEnumerable<Category>> GetCategories()
    {
        // AsNoTracking é usado apenas para somente leitura e melhora a performace, quando aplicado nao rastreia mais o obejro para pegar mudanças
        return await _context.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category> GetCategory(int id)
    {
        // Adicionar um delay de 3 segundos para teste
        // Ele é sincrono e bloqueia a thread
        // System.Threading.Thread.Sleep(3000);
        // ou 
        // await Task.Delay(3000);
        // Quando usar Thread.Sleep? Testes rápidos Debug local (nunca em produção web)
        return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
    }

    public async Task<Category> CreateCategory(Category category)
    {
        if (category is null)
            throw new ArgumentNullException(nameof(category));
        
        _context.Categories.Add(category);
        
        return category;
    }

    public async Task<Category> UpdateCategory(Category category)
    {
        if (category is null)
            throw new ArgumentNullException(nameof(category));

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChangesAsync();
        
        return category;
    }

    public async Task<Category> DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        
        if (category is null)
            throw new ArgumentNullException(nameof(category));

        _context.Categories.Remove(category);
        _context.SaveChangesAsync();
        
        return category;
        
    }

    public async Task<IEnumerable<Category>> GetCategoriesWithProducts()
    {
        return _context.Categories.Include(p => p.Products).ToList();
    }
}