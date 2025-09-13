using MinhaPrimeiraApi.Context;

namespace MinhaPrimeiraApi.Repository;

public class UnitOfWork: IUnitOfWork
{
    private IProductsRepository _productsRepository;
    private ICategoryRepository _categoryRepository;
    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProductsRepository ProductsRepository
    {
        get
        {
            return _productsRepository ??  new ProductsRepository(_context);
        }
        
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepository ?? new CategoriesRepository(_context);
        }
    }

    public async Task Commit()
    {
        _context.SaveChanges();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}