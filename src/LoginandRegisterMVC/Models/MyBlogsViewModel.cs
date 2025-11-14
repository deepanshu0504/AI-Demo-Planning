namespace LoginandRegisterMVC.Models;

public class MyBlogsViewModel
{
    public IEnumerable<Blog> Blogs { get; set; } = new List<Blog>();
    public int TotalCount => Blogs.Count();
    public int PublishedCount => Blogs.Count(b => b.Status == BlogStatus.Published);
    public int DraftCount => Blogs.Count(b => b.Status == BlogStatus.Draft);

    // Filter
    public string Filter { get; set; } = "all"; // all, published, drafts

    // Sort
    public string SortBy { get; set; } = "recent"; // recent, oldest, a-z
}


