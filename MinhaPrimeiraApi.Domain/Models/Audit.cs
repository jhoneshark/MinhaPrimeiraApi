using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaPrimeiraApi.Domain.Models;

public class Audit
{
    [Key]
    public int Id { get; set; }
    
    public int ChangedBy { get; set; }
    
    [ForeignKey("ChangedBy")]
    public virtual Users User { get; set; }
    
    public int AuditableId { get; set; }
    
    [StringLength(191)]
    public string AuditableType { get; set; }
    
    [StringLength(20)]
    public string Event { get; set; }
    
    [Column(TypeName = "longtext")]
    public string OldValues { get; set; }
    
    [Column(TypeName = "longtext")]
    public string NewValues { get; set; }
    
    [StringLength(300)]
    public string Url { get; set; }
    
    [StringLength(50)]
    public string Ip { get; set; }
    
    [Column(TypeName = "longtext")]
    public string UserAgent { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}