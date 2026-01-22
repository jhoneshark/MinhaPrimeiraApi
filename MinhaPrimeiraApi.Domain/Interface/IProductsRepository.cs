using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Models.Pagination;

namespace MinhaPrimeiraApi.Domain.Interface;

public interface IProductsRepository : IGenericRepository<Product>
{
    IQueryable<Product> GetProducts();
    Task<PagedList<Product>> GetProductsPagination(ProductsParameters productsParameters);
    Task<PagedList<Product>> GetProductsFilterPrice(ProductsFilterPrice productsParameters);
    Task<IEnumerable<Product>> GetProductByCategorie(int id);
    Product GetProduct(int id);
    Product CreateProduct(Product product);
    Product UpdateProduct(Product product);
    Product DeleteProduct(int id);
}