using LoginandRegisterMVC.Data;
using LoginandRegisterMVC.Models;
using LoginandRegisterMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LoginandRegisterMVC.Controllers;

public class BlogsController : Controller
{
    private readonly IBlogService _blogService;
    private readonly IImageService _imageService;
    private readonly UserContext _context;
    private readonly ILogger<BlogsController> _logger;

    public BlogsController(
        IBlogService blogService,
        IImageService imageService,
        UserContext context,
        ILogger<BlogsController> logger)
    {
        _blogService = blogService;
        _imageService = imageService;
        _context = context;
        _logger = logger;
    }

    // GET: Blogs or Blogs/Index - Public blogs page with search and filters
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        int pageNumber = 1, 
        int pageSize = 9,
        string? search = null,
        int? categoryId = null,
        string? authorId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string sortBy = "recent")
    {
        try
        {
            var (blogs, totalCount) = await _blogService.GetAllPublishedBlogsAsync(
                pageNumber, 
                pageSize,
                search,
                categoryId,
                authorId,
                startDate,
                endDate,
                sortBy);
            
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var viewModel = new BlogListViewModel
            {
                Blogs = blogs,
                CurrentPage = pageNumber,
                TotalPages = totalPages,
                TotalCount = totalCount,
                PageSize = pageSize
            };

            // Check if this is an AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    blogs = blogs.Select(b => new
                    {
                        blogId = b.BlogId,
                        title = b.Title,
                        excerpt = b.Excerpt ?? (b.Content.Length > 200 ? b.Content.Substring(0, 200) + "..." : b.Content),
                        featuredImage = b.FeaturedImage,
                        categoryName = b.Category?.Name,
                        authorName = b.Author?.Username,
                        publishedDate = b.PublishedDate?.ToString("MMM dd, yyyy"),
                        viewCount = b.ViewCount
                    }),
                    totalCount = totalCount,
                    totalPages = totalPages,
                    currentPage = pageNumber
                });
            }

            // Populate filter options for initial page load
            ViewBag.Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.Authors = await _context.Users
                .Where(u => _context.Blogs.Any(b => b.AuthorId == u.UserId && b.Status == BlogStatus.Published && !b.IsDeleted))
                .OrderBy(u => u.Username)
                .ToListAsync();

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading blogs index");
            
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = false, message = "Error loading blogs" });
            }
            
            return View(new BlogListViewModel());
        }
    }

    // GET: Blogs/Create
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var viewModel = new CreateBlogViewModel
        {
            Categories = await _context.Categories.ToListAsync()
        };

        return View(viewModel);
    }

    // POST: Blogs/Create
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBlogViewModel model, string action)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }

            // Validate and save image
            if (model.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "Please select a featured image");
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }

            if (!_imageService.ValidateImage(model.ImageFile, out string imageError))
            {
                ModelState.AddModelError("ImageFile", imageError);
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }

            // Save image
            var imageName = await _imageService.SaveImageAsync(model.ImageFile, "blogs");

            // Create blog entity
            var blog = new Blog
            {
                Title = model.Title,
                Content = model.Content,
                FeaturedImage = imageName,
                CategoryId = model.CategoryId,
                AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty,
                Status = action == "publish" ? BlogStatus.Published : BlogStatus.Draft
            };

            // Save blog
            var success = await _blogService.CreateBlogAsync(blog);

            if (success)
            {
                if (blog.Status == BlogStatus.Published)
                {
                    TempData["SuccessMessage"] = "Blog published successfully!";
                }
                else
                {
                    TempData["SuccessMessage"] = "Blog saved as draft successfully";
                }

                return RedirectToAction(nameof(MyBlogs));
            }
            else
            {
                ModelState.AddModelError("", "Error saving blog. Please try again.");
                model.Categories = await _context.Categories.ToListAsync();
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating blog");
            ModelState.AddModelError("", "An error occurred while saving the blog.");
            model.Categories = await _context.Categories.ToListAsync();
            return View(model);
        }
    }

    // GET: Blogs/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var blog = await _blogService.GetBlogByIdAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            // Check authorization for draft blogs
            if (blog.Status == BlogStatus.Draft)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                // Only author or admin can view draft blogs
                if (userId != blog.AuthorId && userRole != "Admin")
                {
                    return Forbid();
                }
            }

            // Increment view count (session-based to prevent multiple counts)
            var viewedKey = $"Blog_Viewed_{id}";
            if (HttpContext.Session.GetString(viewedKey) == null)
            {
                await _blogService.IncrementViewCountAsync(id);
                HttpContext.Session.SetString(viewedKey, "true");
            }

            // Get related blogs
            var relatedBlogs = await _blogService.GetRelatedBlogsAsync(id, blog.CategoryId, 3);

            // Check permissions
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);
            var isAuthor = currentUserId == blog.AuthorId;
            var isAdmin = currentUserRole == "Admin";

            var viewModel = new BlogDetailsViewModel
            {
                Blog = blog,
                RelatedBlogs = relatedBlogs,
                IsAuthor = isAuthor,
                IsAdmin = isAdmin,
                CanEdit = isAuthor || isAdmin,
                CanDelete = isAuthor || isAdmin
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading blog details for ID: {id}");
            return NotFound();
        }
    }

    // GET: Blogs/Edit/5
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var blog = await _blogService.GetBlogByIdAsync(id);

            if (blog == null)
            {
                return NotFound();
            }

            // Check authorization - only author or admin can edit
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userId != blog.AuthorId && userRole != "Admin")
            {
                return Forbid();
            }

            var viewModel = new CreateBlogViewModel
            {
                BlogId = blog.BlogId,
                Title = blog.Title,
                Content = blog.Content,
                CategoryId = blog.CategoryId,
                Status = blog.Status,
                ExistingImage = blog.FeaturedImage,
                Categories = await _context.Categories.ToListAsync()
            };

            return View("Create", viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error loading blog for edit: {id}");
            return NotFound();
        }
    }

    // POST: Blogs/Edit/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CreateBlogViewModel model, string action)
    {
        try
        {
            if (id != model.BlogId)
            {
                return NotFound();
            }

            var existingBlog = await _blogService.GetBlogByIdAsync(id);
            if (existingBlog == null)
            {
                return NotFound();
            }

            // Check authorization
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userId != existingBlog.AuthorId && userRole != "Admin")
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await _context.Categories.ToListAsync();
                model.ExistingImage = existingBlog.FeaturedImage;
                return View("Create", model);
            }

            // Handle image update
            string imageName = existingBlog.FeaturedImage;

            if (model.ImageFile != null)
            {
                // Validate new image
                if (!_imageService.ValidateImage(model.ImageFile, out string imageError))
                {
                    ModelState.AddModelError("ImageFile", imageError);
                    model.Categories = await _context.Categories.ToListAsync();
                    model.ExistingImage = existingBlog.FeaturedImage;
                    return View("Create", model);
                }

                // Delete old image
                await _imageService.DeleteImageAsync($"blogs/{existingBlog.FeaturedImage}");

                // Save new image
                imageName = await _imageService.SaveImageAsync(model.ImageFile, "blogs");
            }

            // Update blog entity
            existingBlog.Title = model.Title;
            existingBlog.Content = model.Content;
            existingBlog.FeaturedImage = imageName;
            existingBlog.CategoryId = model.CategoryId;
            existingBlog.Status = action == "publish" ? BlogStatus.Published : model.Status;
            existingBlog.LastUpdatedDate = DateTime.Now;

            // Set PublishedDate if changing from Draft to Published
            if (existingBlog.Status == BlogStatus.Published && !existingBlog.PublishedDate.HasValue)
            {
                existingBlog.PublishedDate = DateTime.Now;
            }

            // Update blog
            var success = await _blogService.UpdateBlogAsync(existingBlog);

            if (success)
            {
                TempData["SuccessMessage"] = "Blog updated successfully!";
                return RedirectToAction(nameof(MyBlogs));
            }
            else
            {
                ModelState.AddModelError("", "Error updating blog. Please try again.");
                model.Categories = await _context.Categories.ToListAsync();
                model.ExistingImage = existingBlog.FeaturedImage;
                return View("Create", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating blog: {id}");
            ModelState.AddModelError("", "An error occurred while updating the blog.");
            model.Categories = await _context.Categories.ToListAsync();
            return View("Create", model);
        }
    }

    // POST: Blogs/Delete/5
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var blog = await _blogService.GetBlogByIdAsync(id);

            if (blog == null)
            {
                return Json(new { success = false, message = "Blog not found" });
            }

            // Check authorization
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userId != blog.AuthorId && userRole != "Admin")
            {
                return Json(new { success = false, message = "You don't have permission to delete this blog" });
            }

            // Delete blog (soft delete)
            var success = await _blogService.DeleteBlogAsync(id);

            if (success)
            {
                // Delete associated images
                await _imageService.DeleteImageAsync($"blogs/{blog.FeaturedImage}");

                TempData["SuccessMessage"] = "Blog deleted successfully!";
                return Json(new { success = true, message = "Blog deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Error deleting blog" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting blog: {id}");
            return Json(new { success = false, message = "An error occurred while deleting the blog" });
        }
    }

    // GET: Blogs/MyBlogs
    [Authorize]
    public async Task<IActionResult> MyBlogs(string filter = "all", string sortBy = "recent")
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Users");
        }

        BlogStatus? statusFilter = filter.ToLower() switch
        {
            "published" => BlogStatus.Published,
            "drafts" => BlogStatus.Draft,
            _ => null
        };

        var blogs = await _blogService.GetBlogsByAuthorAsync(userId, statusFilter);

        // Apply sorting
        blogs = sortBy.ToLower() switch
        {
            "oldest" => blogs.OrderBy(b => b.CreatedDate),
            "a-z" => blogs.OrderBy(b => b.Title),
            _ => blogs.OrderByDescending(b => b.LastUpdatedDate)
        };

        var viewModel = new MyBlogsViewModel
        {
            Blogs = blogs,
            Filter = filter,
            SortBy = sortBy
        };

        return View(viewModel);
    }
}

