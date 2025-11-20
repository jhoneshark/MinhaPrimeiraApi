using MinhaPrimeiraApi.Domain.Models;
using MinhaPrimeiraApi.Domain.Models.Pagination;
using X.PagedList;

namespace MinhaPrimeiraApi.Domain.Interface;

public interface ICategoryRepository
{
    // esse IPagedList é uma biblioteca
    Task<IPagedList<Category>> GetCategoriesPagination(CategoriesParameters categoriesParameters);
    Task<Models.Pagination.PagedList<Category>> GetCategoriesFilterName(CategoriesFilterName categoriesParameters);
    Task<IEnumerable<Category>> GetCategories();
    Category GetCategory(int id);
    Category CreateCategory(Category category);
    Category UpdateCategory(Category category);
    Category DeleteCategory(int id);
    IEnumerable<Category> GetCategoriesWithProducts();
}