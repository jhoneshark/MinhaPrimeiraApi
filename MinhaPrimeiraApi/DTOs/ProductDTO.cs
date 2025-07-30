using System.ComponentModel.DataAnnotations;
using MinhaPrimeiraApi.Validations;

namespace MinhaPrimeiraApi.DTOs;

public class ProductDTO
{
    public int ProductId { get; set; }
    
    [Microsoft.Build.Framework.Required]
    [StringLength(80, MinimumLength = 2)]
    [PrimeiraLetraMaiuscula]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Informe uma descrição para o produto")]
    [StringLength(300)]
    public string? Description { get; set; }
    
    [Microsoft.Build.Framework.Required]
    public decimal Price { get; set; }
    
    [Microsoft.Build.Framework.Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
}