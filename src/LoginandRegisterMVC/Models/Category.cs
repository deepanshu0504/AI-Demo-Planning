using System.ComponentModel.DataAnnotations;

namespace LoginandRegisterMVC.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Category Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(255)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required]
    [StringLength(60)]
    public string Slug { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    // Navigation Property
    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}


