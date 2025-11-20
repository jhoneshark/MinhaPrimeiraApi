namespace MinhaPrimeiraApi.Domain.Interface;

public interface IUnitOfWork
{
    IProductsRepository ProductsRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    
    Task Commit();
}