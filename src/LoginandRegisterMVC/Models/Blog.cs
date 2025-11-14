using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginandRegisterMVC.Models;

public class Blog
{
    [Key]
    public int BlogId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters")]
    [Display(Name = "Blog Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(10000, MinimumLength = 50, ErrorMessage = "Content must be between 50 and 10,000 characters")]
    [Display(Name = "Blog Content")]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; } = string.Empty;

    [StringLength(250)]
    public string? Excerpt { get; set; }

    [Required]
    [Display(Name = "Featured Image")]
    public string FeaturedImage { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string AuthorId { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    [Required]
    public BlogStatus Status { get; set; } = BlogStatus.Draft;

    public int ViewCount { get; set; } = 0;

    public DateTime CreatedDate { get; set; }

    public DateTime? PublishedDate { get; set; }

    public DateTime LastUpdatedDate { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    [ForeignKey("AuthorId")]
    public virtual User? Author { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }

    public virtual ICollection<BlogTag> BlogTags { get; set; } = new List<BlogTag>();

    // NotMapped property for image upload
    [NotMapped]
    [Display(Name = "Upload Image")]
    public IFormFile? ImageFile { get; set; }
}


