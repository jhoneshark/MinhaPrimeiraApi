using MinhaPrimeiraApi.Models;
using MinhaPrimeiraApi.Models.Pagination;

namespace MinhaPrimeiraApi.Repository;

public interface ICategoryRepository
{
    Task<PagedList<Category>> GetCategoriesPagination(CategoriesParameters categoriesParameters);
    Task<PagedList<Category>> GetCategoriesFilterName(CategoriesFilterName categoriesParameters);
    Task<IEnumerable<Category>> GetCategories();
    Category GetCategory(int id);
    Category CreateCategory(Category category);
    Category UpdateCategory(Category category);
    Category DeleteCategory(int id);
    IEnumerable<Category> GetCategoriesWithProducts();
}