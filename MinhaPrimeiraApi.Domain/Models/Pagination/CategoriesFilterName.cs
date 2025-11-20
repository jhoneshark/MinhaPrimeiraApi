namespace MinhaPrimeiraApi.Domain.Models.Pagination;

public class CategoriesFilterName : QueryStringParameters
{
    public string? Name { get; set; }
}