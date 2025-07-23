using MinhaPrimeiraApi.Models;

namespace MinhaPrimeiraApi.Repository;

public interface ICategoryRepository
{
    IEnumerable<Category> GetCategories();
    Category GetCategory(int id);
    Category CreateCategory(Category category);
    Category UpdateCategory(Category category);
    Category DeleteCategory(int id);
    IEnumerable<Category> GetCategoriesWithProducts();
}