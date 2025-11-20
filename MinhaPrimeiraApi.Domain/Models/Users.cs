using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaPrimeiraApi.Domain.Models;

public class Users
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string? CprOrCnpj { get; set; }
    
    [StringLength(15)]
    public string? Name { get; set; }
    
    [StringLength(20)]
    public string? Phone { get; set; }
    
    [StringLength(200)]
    public string? Email { get; set; }
    
    [StringLength(200)]
    public string PasswordHash { get; set; }
    
    [Required]
    public int RoleId { get; set; }
    
    [ForeignKey("RoleId")]
    public Roles Role { get; set; }
    
    [Required]
    public string RefreshToekn { get; set; }
    
    [Required]
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}