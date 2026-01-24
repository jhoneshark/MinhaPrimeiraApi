using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Domain.Interface;
using MinhaPrimeiraApi.Infra.Context;
using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Models.Pagination;

namespace MinhaPrimeiraApi.Domain.Repository;

public class ProductsRepository : IProductsRepository
{
    private readonly AppDbContext _context;

    public ProductsRepository(AppDbContext context)
    {
        _context = context;
    }

    // Paginação simples
    // public IEnumerable<Product> GetProductsPagination(ProductsParameters productsParameters)
    // {
    //     return GetProducts()
    //         .OrderBy(p => p.Name)
    //         .Skip((productsParameters.PageNumber - 1) * productsParameters.PageSize)
    //         .Take(productsParameters.PageSize)
    //         .ToList();
    // }

    // Paginação completa
    public async Task<PagedList<Product>> GetProductsPagination(ProductsParameters productsParameters)
    {
        var productQuery = _context.Products.OrderBy(p => p.CategoryId).AsQueryable();
        var productsOrdered = await PagedList<Product>.ToPagedList(productQuery, productsParameters.PageNumber, productsParameters.PageSize);
        return productsOrdered;
    }

    public async Task<PagedList<Product>> GetProductsFilterPrice(ProductsFilterPrice productsParameters)
    {
        var productQuery = GetProducts().OrderBy(p => p.Price).AsQueryable();

        if (productsParameters.Price.HasValue && !string.IsNullOrEmpty(productsParameters.PriceCriteria))
        {
            if (productsParameters.PriceCriteria.Equals("maior", StringComparison.InvariantCultureIgnoreCase))
            {
                productQuery = productQuery.Where(p => p.Price > productsParameters.Price.Value);
            } 
            else if (productsParameters.PriceCriteria.Equals("menor", StringComparison.InvariantCultureIgnoreCase))
            {
                productQuery = productQuery.Where(p => p.Price < productsParameters.Price.Value);
            } 
            else if (productsParameters.PriceCriteria.Equals("igual", StringComparison.InvariantCultureIgnoreCase))
            {
                productQuery = productQuery.Where(p => p.Price == productsParameters.Price.Value);
            }
        }
        
        var productsFiltered = await PagedList<Product>.ToPagedList(productQuery, productsParameters.PageNumber, productsParameters.PageSize);
        
        return productsFiltered;
    }

    public async Task<IEnumerable<Product>> GetProductByCategorie(int id)
    {
        return await _context.Products.Where(p => p.CategoryId == id).AsNoTracking().ToListAsync();
    }

    public IQueryable<Product> GetProducts()
    {
        return _context.Products;
    }

    public async Task<Product> GetProduct(int id)
    {
        return _context.Products.FirstOrDefault(p => p.ProductId == id);
    }

    public async Task<Product> CreateProduct(Product product)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));
        
        _context.Products.Add(product);
        _context.SaveChangesAsync();
        
        return product;
    }

    public async Task<Product> UpdateProduct(Product product)
    {
        if  (product is null)
            throw new ArgumentNullException(nameof(product));

        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChangesAsync();
        
        return product;
    }

    public async Task<Product> DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);

        if (product is null)
            throw new ArgumentNullException(nameof(product));
        
        _context.Products.Remove(product);
        _context.SaveChangesAsync();
        
        return product;
    }
}