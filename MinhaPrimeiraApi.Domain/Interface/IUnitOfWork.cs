namespace MinhaPrimeiraApi.Domain.Interface;

public interface IUnitOfWork : IDisposable
{
    IProductsRepository ProductsRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    Task Commit();
}