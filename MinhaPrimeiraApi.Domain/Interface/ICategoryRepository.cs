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
    Task<Category> GetCategory(int id);
    Task<Category> CreateCategory(Category category);
    Task<Category> UpdateCategory(Category category);
    Task<Category> DeleteCategory(int id);
    Task<IEnumerable<Category>> GetCategoriesWithProducts();
}