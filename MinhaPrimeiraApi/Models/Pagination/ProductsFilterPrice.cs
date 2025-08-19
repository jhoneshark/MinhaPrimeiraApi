namespace MinhaPrimeiraApi.Models.Pagination;

public class ProductsFilterPrice : QueryStringParameters
{
    public decimal? Price { get; set; }
    public string? PriceCriteria { get; set; } // maior, menor ou igual
}