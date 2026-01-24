using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Models.Pagination;

namespace MinhaPrimeiraApi.Domain.Interface;

public interface IProductsRepository
{
    IQueryable<Product> GetProducts();
    
    // Paginaçao simples
    // IEnumerable<Product> GetProductsPagination(ProductsParameters productsParameters );
    // Pagianção completa
    Task<PagedList<Product>> GetProductsPagination(ProductsParameters productsParameters);
    Task<PagedList<Product>> GetProductsFilterPrice(ProductsFilterPrice productsParameters);
    Task<IEnumerable<Product>> GetProductByCategorie(int id);
    Task<Product> GetProduct(int id);
    Task<Product> CreateProduct(Product product);
    Task<Product> UpdateProduct(Product product);
    Task<Product> DeleteProduct(int id);
}