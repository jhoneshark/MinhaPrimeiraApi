using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Models.Pagination;

namespace MinhaPrimeiraApi.Repository;

public interface IProductsRepository
{
    IQueryable<Product> GetProducts();
    
    // Paginaçao simples
    // IEnumerable<Product> GetProductsPagination(ProductsParameters productsParameters );
    // Pagianção completa
    Task<PagedList<Product>> GetProductsPagination(ProductsParameters productsParameters);
    Task<PagedList<Product>> GetProductsFilterPrice(ProductsFilterPrice productsParameters);
    Task<IEnumerable<Product>> GetProductByCategorie(int id);
    Product GetProduct(int id);
    Product CreateProduct(Product product);
    Product UpdateProduct(Product product);
    Product DeleteProduct(int id);
}