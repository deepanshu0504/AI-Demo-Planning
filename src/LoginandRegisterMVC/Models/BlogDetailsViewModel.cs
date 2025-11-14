namespace LoginandRegisterMVC.Models;

public class BlogDetailsViewModel
{
    public Blog Blog { get; set; } = null!;
    public IEnumerable<Blog> RelatedBlogs { get; set; } = new List<Blog>();
    public bool IsAuthor { get; set; }
    public bool IsAdmin { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}


