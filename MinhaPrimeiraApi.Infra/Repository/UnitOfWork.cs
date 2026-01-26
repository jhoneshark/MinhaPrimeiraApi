using MinhaPrimeiraApi.Infra.Context;
using MinhaPrimeiraApi.Domain.Interface;

namespace MinhaPrimeiraApi.Domain.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    // Repositórios injetados
    public IProductsRepository ProductsRepository { get; }
    public ICategoryRepository CategoryRepository { get; }

    public UnitOfWork(AppDbContext context, 
        IProductsRepository productsRepository, 
        ICategoryRepository categoryRepository)
    {
        _context = context;
        ProductsRepository = productsRepository;
        CategoryRepository = categoryRepository;
    }

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}