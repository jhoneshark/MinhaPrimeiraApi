using System.ComponentModel.DataAnnotations;

namespace MinhaPrimeiraApi.Domain.DTOs;

public class LoginModelDTO
{
    [Required(ErrorMessage = "Informe o email do usuario")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Informe a senha do usuario")]
    public string Password { get; set; }
}