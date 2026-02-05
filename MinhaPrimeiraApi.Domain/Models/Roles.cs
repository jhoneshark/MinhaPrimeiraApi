using System.ComponentModel.DataAnnotations;
using MinhaPrimeiraApi.Domain.Interface;

namespace MinhaPrimeiraApi.Domain.Models;

public class Roles : IAudiTable
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(100)]
    public string? Description { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Users> Users { get; set; } = new List<Users>();
}