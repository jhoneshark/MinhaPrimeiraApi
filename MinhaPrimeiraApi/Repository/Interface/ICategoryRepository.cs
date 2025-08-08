using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Models.Pagination;

namespace MinhaPrimeiraApi.Repository;

public interface ICategoryRepository
{
    PagedList<Category> GetCategoriesPagination(CategoriesParameters categoriesParameters);
    IEnumerable<Category> GetCategories();
    Category GetCategory(int id);
    Category CreateCategory(Category category);
    Category UpdateCategory(Category category);
    Category DeleteCategory(int id);
    IEnumerable<Category> GetCategoriesWithProducts();
}