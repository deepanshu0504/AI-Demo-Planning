using LoginandRegisterMVC.Models;

namespace LoginandRegisterMVC.Services;

public interface IBlogService
{
    /// <summary>
    /// Gets all published blogs with pagination, search, and filters
    /// </summary>
    Task<(IEnumerable<Blog> blogs, int totalCount)> GetAllPublishedBlogsAsync(
        int pageNumber = 1, 
        int pageSize = 9,
        string? searchQuery = null,
        int? categoryId = null,
        string? authorId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string sortBy = "recent");

    /// <summary>
    /// Gets a blog by ID with related entities
    /// </summary>
    Task<Blog?> GetBlogByIdAsync(int id);

    /// <summary>
    /// Gets a blog by its SEO-friendly slug
    /// </summary>
    Task<Blog?> GetBlogBySlugAsync(string slug);

    /// <summary>
    /// Gets all blogs by a specific author
    /// </summary>
    Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(string authorId, BlogStatus? status = null);

    /// <summary>
    /// Creates a new blog post
    /// </summary>
    Task<bool> CreateBlogAsync(Blog blog);

    /// <summary>
    /// Updates an existing blog post
    /// </summary>
    Task<bool> UpdateBlogAsync(Blog blog);

    /// <summary>
    /// Soft deletes a blog post
    /// </summary>
    Task<bool> DeleteBlogAsync(int id);

    /// <summary>
    /// Increments the view count for a blog
    /// </summary>
    Task IncrementViewCountAsync(int id);

    /// <summary>
    /// Generates a SEO-friendly slug from a title
    /// </summary>
    string GenerateSlug(string title);

    /// <summary>
    /// Generates an excerpt from content
    /// </summary>
    string GenerateExcerpt(string content, int length = 150);

    /// <summary>
    /// Gets related blogs based on category
    /// </summary>
    Task<IEnumerable<Blog>> GetRelatedBlogsAsync(int blogId, int? categoryId, int count = 3);

    /// <summary>
    /// Search and filter blogs
    /// </summary>
    Task<(IEnumerable<Blog> blogs, int totalCount)> SearchBlogsAsync(
        string? searchQuery = null,
        int? categoryId = null,
        string? authorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "recent",
        int pageNumber = 1,
        int pageSize = 9);
}

