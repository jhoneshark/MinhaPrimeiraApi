using Microsoft.EntityFrameworkCore;
using MinhaPrimeiraApi.Context;
using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Models.Pagination;

namespace MinhaPrimeiraApi.Repository;

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
    public PagedList<Product> GetProductsPagination(ProductsParameters productsParameters)
    {
        var productQuery = GetProducts().OrderBy(p => p.CategoryId).AsQueryable();
        var productsOrdered = PagedList<Product>.ToPagedList(productQuery, productsParameters.PageNumber, productsParameters.PageSize);
        return productsOrdered;
    }

    public IEnumerable<Product> GetProductByCategorie(int id)
    {
        return _context.Products.Where(p => p.CategoryId == id);
    }

    public IQueryable<Product> GetProducts()
    {
        return _context.Products;
    }

    public Product GetProduct(int id)
    {
        return _context.Products.FirstOrDefault(p => p.ProductId == id);
    }

    public Product CreateProduct(Product product)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));
        
        _context.Products.Add(product);
        _context.SaveChanges();
        
        return product;
    }

    public Product UpdateProduct(Product product)
    {
        if  (product is null)
            throw new ArgumentNullException(nameof(product));

        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();
        
        return product;
    }

    public Product DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);

        if (product is null)
            throw new ArgumentNullException(nameof(product));
        
        _context.Products.Remove(product);
        _context.SaveChanges();
        
        return product;
    }
}