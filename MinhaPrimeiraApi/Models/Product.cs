using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.MicrosoftExtensions;
using MinhaPrimeiraApi.Validations;

namespace MinhaPrimeiraApi.Models;

public class Product
{
    [Key]
    public int ProductId { get; set; }
    
    [Required]
    [StringLength(80, MinimumLength = 2)]
    [PrimeiraLetraMaiuscula]
    public string? Name { get; set; }
    
    [Required(ErrorMessage = "Informe uma descrição para o produto")]
    [StringLength(300)]
    public string? Description { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }
    
    [Required]
    [StringLength(300)]
    public string? ImageUrl { get; set; }
    
    public float Stock { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }
    
    [JsonIgnore]
    public Category? Category { get; set; }
    
    // Exemplo de data annotations
    // [Range(18, 65)]
    // [Range(18, 65, ErrorMessage = "A Idade deve estar em 18 e 65")]
    // [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Informe um email valido..")]
    // [CreditCard(ErrorMessage = "Informe um credito valido..")]
    // [Url]
    // [Phone]
    // exemplo
    // [Compare"Senha"]
}