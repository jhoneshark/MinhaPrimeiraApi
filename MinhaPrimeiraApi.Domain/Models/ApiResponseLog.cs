using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinhaPrimeiraApi.Domain.Models;

public class ApiResponseLog
{
    [Key]
    public int Id { get; set; }
    
    [StringLength(100)]
    public string Method { get; set; }
    
    [StringLength(300)]
    public string Url { get; set; }
    
    [Column(TypeName = "longtext")]
    public string RequestHeaders { get; set; }
    
    [Column(TypeName = "longtext")]
    public string RequestBody { get; set; }
    
    [Range(0, 9999)]
    public int Status { get; set; }
    
    [Column(TypeName = "longtext")]
    public string ResponseHeaders { get; set; }
    
    [Column(TypeName = "longtext")]
    public string ResponseBody { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}