using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginandRegisterMVC.Models;

public class BlogTag
{
    [Required]
    public int BlogId { get; set; }

    [Required]
    public int TagId { get; set; }

    // Navigation Properties
    [ForeignKey("BlogId")]
    public virtual Blog Blog { get; set; } = null!;

    [ForeignKey("TagId")]
    public virtual Tag Tag { get; set; } = null!;
}


