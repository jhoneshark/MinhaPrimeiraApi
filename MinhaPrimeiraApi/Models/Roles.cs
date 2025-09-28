using System.ComponentModel.DataAnnotations;

namespace MinhaPrimeiraApi.Models;

public class Roles
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(100)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Users> Users { get; set; } = new List<Users>();
}