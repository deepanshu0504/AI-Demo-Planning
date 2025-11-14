using System.ComponentModel.DataAnnotations;

namespace LoginandRegisterMVC.Models;

public class Tag
{
    [Key]
    public int TagId { get; set; }

    [Required]
    [StringLength(30)]
    [Display(Name = "Tag Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(40)]
    public string Slug { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    // Navigation Property
    public virtual ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();
}


