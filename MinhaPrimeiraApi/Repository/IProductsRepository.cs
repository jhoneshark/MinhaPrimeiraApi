using MinhaPrimeiraApi.Models;

namespace MinhaPrimeiraApi.Repository;

public interface IProductsRepository
{
    IQueryable<Product> GetProducts();
    Product GetProduct(int id);
    Product CreateProduct(Product product);
    Product UpdateProduct(Product product);
    Product DeleteProduct(int id);
}