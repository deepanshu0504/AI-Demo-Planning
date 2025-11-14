using System.ComponentModel.DataAnnotations;

namespace LoginandRegisterMVC.Models;

public class CreateBlogViewModel
{
    public int? BlogId { get; set; } // Null for create, populated for edit

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 100 characters")]
    [Display(Name = "Blog Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(10000, MinimumLength = 50, ErrorMessage = "Content must be between 50 and 10,000 characters")]
    [Display(Name = "Blog Content")]
    [DataType(DataType.MultilineText)]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Featured Image")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Current Image")]
    public string? ExistingImage { get; set; }

    [Display(Name = "Category")]
    public int? CategoryId { get; set; }

    [Display(Name = "Status")]
    public BlogStatus Status { get; set; } = BlogStatus.Draft;

    public IEnumerable<Category>? Categories { get; set; }

    // For displaying character counts
    public int TitleLength => Title?.Length ?? 0;
    public int ContentLength => Content?.Length ?? 0;

    public bool IsEdit => BlogId.HasValue && BlogId.Value > 0;
}


