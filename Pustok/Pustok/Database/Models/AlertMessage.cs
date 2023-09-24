using Pustok.Database.Base;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Pustok.Database.Models;

[Table("AlertMessage")]
public class AlertMessage : BaseEntity<int>, IAuditable
{
    public string Title { get; set; }
    public string Content { get; set; }
    

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
