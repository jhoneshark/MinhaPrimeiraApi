using System.ComponentModel.DataAnnotations;

namespace MinhaPrimeiraApi.Domain.DTOs;

public class RegisterModelDTO
{
    [EmailAddress]
    [Required(ErrorMessage = "E-mail is required")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}