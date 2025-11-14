using LoginandRegisterMVC.Data;
using LoginandRegisterMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LoginandRegisterMVC.Services;

public class BlogService : IBlogService
{
    private readonly UserContext _context;
    private readonly ILogger<BlogService> _logger;

    public BlogService(UserContext context, ILogger<BlogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(IEnumerable<Blog> blogs, int totalCount)> GetAllPublishedBlogsAsync(
        int pageNumber = 1, 
        int pageSize = 9,
        string? searchQuery = null,
        int? categoryId = null,
        string? authorId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string sortBy = "recent")
    {
        try
        {
            var query = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Status == BlogStatus.Published && !b.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Content.Contains(searchQuery) ||
                    (b.Author != null && b.Author.Username.Contains(searchQuery)));
            }

            // Apply category filter
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            // Apply author filter
            if (!string.IsNullOrWhiteSpace(authorId))
            {
                query = query.Where(b => b.AuthorId == authorId);
            }

            // Apply date range filter
            if (startDate.HasValue)
            {
                query = query.Where(b => b.PublishedDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Add one day to include the end date
                var endDateInclusive = endDate.Value.AddDays(1);
                query = query.Where(b => b.PublishedDate < endDateInclusive);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "oldest" => query.OrderBy(b => b.PublishedDate),
                "views" => query.OrderByDescending(b => b.ViewCount),
                "a-z" => query.OrderBy(b => b.Title),
                "z-a" => query.OrderByDescending(b => b.Title),
                _ => query.OrderByDescending(b => b.PublishedDate) // "recent" is default
            };

            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting published blogs");
            return (Enumerable.Empty<Blog>(), 0);
        }
    }

    public async Task<Blog?> GetBlogByIdAsync(int id)
    {
        try
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BlogTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.BlogId == id && !b.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting blog by ID: {id}");
            return null;
        }
    }

    public async Task<Blog?> GetBlogBySlugAsync(string slug)
    {
        try
        {
            return await _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Include(b => b.BlogTags)
                    .ThenInclude(bt => bt.Tag)
                .FirstOrDefaultAsync(b => b.Slug == slug && !b.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting blog by slug: {slug}");
            return null;
        }
    }

    public async Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(string authorId, BlogStatus? status = null)
    {
        try
        {
            var query = _context.Blogs
                .Include(b => b.Category)
                .Where(b => b.AuthorId == authorId && !b.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(b => b.Status == status.Value);
            }

            return await query
                .OrderByDescending(b => b.LastUpdatedDate)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting blogs by author: {authorId}");
            return Enumerable.Empty<Blog>();
        }
    }

    public async Task<bool> CreateBlogAsync(Blog blog)
    {
        try
        {
            blog.CreatedDate = DateTime.Now;
            blog.LastUpdatedDate = DateTime.Now;
            blog.Slug = GenerateUniqueSlug(blog.Title);
            blog.Excerpt = GenerateExcerpt(blog.Content);

            if (blog.Status == BlogStatus.Published)
            {
                blog.PublishedDate = DateTime.Now;
            }

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Blog created successfully: {blog.Title}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog");
            return false;
        }
    }

    public async Task<bool> UpdateBlogAsync(Blog blog)
    {
        try
        {
            var existingBlog = await _context.Blogs.FindAsync(blog.BlogId);
            if (existingBlog == null)
            {
                return false;
            }

            existingBlog.Title = blog.Title;
            existingBlog.Content = blog.Content;
            existingBlog.Excerpt = GenerateExcerpt(blog.Content);
            existingBlog.CategoryId = blog.CategoryId;
            existingBlog.LastUpdatedDate = DateTime.Now;

            // Update FeaturedImage only if a new image is provided
            if (!string.IsNullOrEmpty(blog.FeaturedImage) && blog.FeaturedImage != existingBlog.FeaturedImage)
            {
                existingBlog.FeaturedImage = blog.FeaturedImage;
            }

            // If changing from Draft to Published, set PublishedDate
            if (existingBlog.Status == BlogStatus.Draft && blog.Status == BlogStatus.Published && !existingBlog.PublishedDate.HasValue)
            {
                existingBlog.PublishedDate = DateTime.Now;
            }

            existingBlog.Status = blog.Status;

            // Update slug if title changed
            if (existingBlog.Title != blog.Title)
            {
                existingBlog.Slug = GenerateUniqueSlug(blog.Title);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Blog updated successfully: {blog.Title}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating blog: {blog.BlogId}");
            return false;
        }
    }

    public async Task<bool> DeleteBlogAsync(int id)
    {
        try
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return false;
            }

            // Soft delete
            blog.IsDeleted = true;
            blog.LastUpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Blog deleted successfully: {id}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting blog: {id}");
            return false;
        }
    }

    public async Task IncrementViewCountAsync(int id)
    {
        try
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                blog.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error incrementing view count for blog: {id}");
        }
    }

    public string GenerateSlug(string title)
    {
        if (string.IsNullOrEmpty(title))
            return string.Empty;

        // Convert to lowercase
        var slug = title.ToLowerInvariant();

        // Remove special characters
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

        // Replace multiple spaces or hyphens with single hyphen
        slug = Regex.Replace(slug, @"[\s-]+", "-");

        // Trim hyphens from start and end
        slug = slug.Trim('-');

        // Limit length
        if (slug.Length > 100)
        {
            slug = slug.Substring(0, 100).Trim('-');
        }

        return slug;
    }

    private string GenerateUniqueSlug(string title)
    {
        var slug = GenerateSlug(title);
        var originalSlug = slug;
        var counter = 1;

        while (_context.Blogs.Any(b => b.Slug == slug))
        {
            slug = $"{originalSlug}-{counter}";
            counter++;
        }

        return slug;
    }

    public string GenerateExcerpt(string content, int length = 150)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;

        // Remove HTML tags if any
        var plainText = Regex.Replace(content, "<.*?>", string.Empty);

        // Trim to length
        if (plainText.Length <= length)
            return plainText;

        // Find last space before length to avoid cutting words
        var excerpt = plainText.Substring(0, length);
        var lastSpace = excerpt.LastIndexOf(' ');

        if (lastSpace > 0)
        {
            excerpt = excerpt.Substring(0, lastSpace);
        }

        return excerpt + "...";
    }

    public async Task<IEnumerable<Blog>> GetRelatedBlogsAsync(int blogId, int? categoryId, int count = 3)
    {
        try
        {
            var query = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.BlogId != blogId && b.Status == BlogStatus.Published && !b.IsDeleted);

            // Prefer blogs from same category
            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            var relatedBlogs = await query
                .OrderByDescending(b => b.PublishedDate)
                .Take(count)
                .ToListAsync();

            // If not enough blogs in same category, fill with recent blogs
            if (relatedBlogs.Count < count)
            {
                var additionalCount = count - relatedBlogs.Count;
                var additionalBlogs = await _context.Blogs
                    .Include(b => b.Author)
                    .Include(b => b.Category)
                    .Where(b => b.BlogId != blogId && b.Status == BlogStatus.Published && !b.IsDeleted && !relatedBlogs.Select(rb => rb.BlogId).Contains(b.BlogId))
                    .OrderByDescending(b => b.PublishedDate)
                    .Take(additionalCount)
                    .ToListAsync();

                relatedBlogs.AddRange(additionalBlogs);
            }

            return relatedBlogs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting related blogs for: {blogId}");
            return Enumerable.Empty<Blog>();
        }
    }

    public async Task<(IEnumerable<Blog> blogs, int totalCount)> SearchBlogsAsync(
        string? searchQuery = null,
        int? categoryId = null,
        string? authorId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string sortBy = "recent",
        int pageNumber = 1,
        int pageSize = 9)
    {
        try
        {
            var query = _context.Blogs
                .Include(b => b.Author)
                .Include(b => b.Category)
                .Where(b => b.Status == BlogStatus.Published && !b.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Content.Contains(searchQuery) ||
                    (b.Author != null && b.Author.Username.Contains(searchQuery)));
            }

            // Apply category filter
            if (categoryId.HasValue)
            {
                query = query.Where(b => b.CategoryId == categoryId.Value);
            }

            // Apply author filter
            if (!string.IsNullOrWhiteSpace(authorId))
            {
                query = query.Where(b => b.AuthorId == authorId);
            }

            // Apply date range filter
            if (fromDate.HasValue)
            {
                query = query.Where(b => b.PublishedDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(b => b.PublishedDate <= toDate.Value);
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "oldest" => query.OrderBy(b => b.PublishedDate),
                "mostviewed" => query.OrderByDescending(b => b.ViewCount),
                "a-z" => query.OrderBy(b => b.Title),
                "z-a" => query.OrderByDescending(b => b.Title),
                _ => query.OrderByDescending(b => b.PublishedDate) // recent
            };

            var totalCount = await query.CountAsync();
            var blogs = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (blogs, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching blogs");
            return (Enumerable.Empty<Blog>(), 0);
        }
    }
}

