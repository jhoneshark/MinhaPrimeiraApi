using System.ComponentModel.DataAnnotations;
namespace MinhaPrimeiraApi.Domain.DTOs;

public class CategoryDTO
{
    [StringLength(80)]
    public string? Name { get; set; }
    
    // Para ignorar algo pode usar a [JsonIgnore]
    [StringLength(300)]
    public string? ImageUrl { get; set; }
}