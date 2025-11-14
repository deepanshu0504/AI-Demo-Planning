namespace LoginandRegisterMVC.Models;

public class BlogListViewModel
{
    public IEnumerable<Blog> Blogs { get; set; } = new List<Blog>();
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; } = 9;
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;

    // Search and filter properties
    public string? SearchQuery { get; set; }
    public int? CategoryId { get; set; }
    public string? AuthorId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string SortBy { get; set; } = "recent";

    // For filter dropdowns
    public IEnumerable<Category>? Categories { get; set; }
    public IEnumerable<User>? Authors { get; set; }
}


