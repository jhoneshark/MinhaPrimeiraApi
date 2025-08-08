using System.ComponentModel.DataAnnotations;
namespace MinhaPrimeiraApi.DTOs;

public class CategoryDTO
{
    [StringLength(80)]
    public string? Name { get; set; }
    
    [StringLength(300)]
    public string? ImageUrl { get; set; }
}