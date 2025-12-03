using System.ComponentModel.DataAnnotations;

namespace MinhaPrimeiraApi.Domain.DTOs;

public class RegisterModelDTO
{
    [Required]
    [StringLength(15)]
    public string? Name { get; set; }
    
    [Required]
    [StringLength(200)]
    public string? CprOrCnpj { get; set; }
    
    [EmailAddress]
    [Required(ErrorMessage = "E-mail is required")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}