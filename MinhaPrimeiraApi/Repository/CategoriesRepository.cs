using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Models.Pagination;

namespace MinhaPrimeiraApi.Repository;

public class CategoriesRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoriesRepository(AppDbContext context)
    {
        _context = context;
    }

    public PagedList<Category> GetCategoriesPagination(CategoriesParameters categoriesParameters)
    {
        var categoriesQuery = GetCategories().OrderBy(p => p.CategoryId).AsQueryable();
        var categoriesOrdered = PagedList<Category>.ToPagedList(categoriesQuery, categoriesParameters.PageNumber, categoriesParameters.PageSize);
        return categoriesOrdered;
    }

    public PagedList<Category> GetCategoriesFilterName(CategoriesFilterName categoriesParameters)
    {
        var categories = GetCategories().OrderBy(p => p.Name).AsQueryable();

        if (!string.IsNullOrEmpty(categoriesParameters.Name))
        {
            categories = categories.Where(p => p.Name.ToLower().Contains(categoriesParameters.Name.ToLower()));
        }

        var categoriesFiltered = PagedList<Category>.ToPagedList(categories, categoriesParameters.PageNumber, categoriesParameters.PageSize);
        
        return categoriesFiltered;
    }

    public IEnumerable<Category> GetCategories()
    {
        // AsNoTracking é usado apenas para somente leitura e melhora a performace, quando aplicado nao rastreia mais o obejro para pegar mudanças
        return _context.Categories.AsNoTracking().ToList();
    }

    public Category GetCategory(int id)
    {
        return _context.Categories.FirstOrDefault(c => c.CategoryId == id);
    }

    public Category CreateCategory(Category category)
    {
        if (category is null)
            throw new ArgumentNullException(nameof(category));
        
        _context.Categories.Add(category);
        _context.SaveChanges();
        
        return category;
    }

    public Category UpdateCategory(Category category)
    {
        if (category is null)
            throw new ArgumentNullException(nameof(category));

        _context.Entry(category).State = EntityState.Modified;
        _context.SaveChanges();
        
        return category;
    }

    public Category DeleteCategory(int id)
    {
        var category = _context.Categories.Find(id);
        
        if (category is null)
            throw new ArgumentNullException(nameof(category));

        _context.Categories.Remove(category);
        _context.SaveChanges();
        
        return category;
        
    }

    public IEnumerable<Category> GetCategoriesWithProducts()
    {
        return _context.Categories.Include(p => p.Products).ToList();
    }
}