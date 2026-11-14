using System.ComponentModel.DataAnnotations;

namespace CareerFit.Models;

public class CareerCategory
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string IconClass { get; set; } = "bi-briefcase";

    public int DisplayOrder { get; set; } = 0;

    // Navigation properties
    public virtual ICollection<Career> Careers { get; set; } = new List<Career>();
}
