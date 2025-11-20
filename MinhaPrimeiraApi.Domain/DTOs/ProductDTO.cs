using System.ComponentModel.DataAnnotations;
using MinhaPrimeiraApi.Domain.Validations;

namespace MinhaPrimeiraApi.Domain.DTOs;

public class ProductDTO
{
    public int ProductId { get; set; }
    
    [Required(ErrorMessage = "O Nome é obrigatório")] 
    [StringLength(80, MinimumLength = 2)]
    [PrimeiraLetraMaiuscula]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Informe uma descrição para o produto")]
    [StringLength(300)]
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "O Preço é obrigatório")] 
    public decimal Price { get; set; }
    
    [Required(ErrorMessage = "A URL da Imagem é obrigatória")] 
    [StringLength(300)]
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
}