# Blog Management Feature - User Stories

## 📋 Document Information
**Project**: Login and Register MVC Application  
**Feature**: Blog Management System  
**Version**: 1.1  
**Created**: November 13, 2024  
**Last Updated**: November 13, 2024  
**Technology Stack**: ASP.NET Core MVC 8, Entity Framework Core, SQL Server, Bootstrap 4.5.2

---

## 🎯 Feature Overview

This document outlines the user stories for implementing a comprehensive Blog Management System in the existing ASP.NET Core MVC application. The feature will allow users to create, manage, and view blog posts with images, while maintaining proper validation and user experience standards.

### Key Objectives
- Enable users to create and publish blog posts
- Support image uploads with blogs
- Implement draft and publish workflow
- Display blogs on a dedicated main blogs page accessible via menu
- Provide blog management capabilities for users and admins
- Integrate blogs seamlessly into the main site navigation

---

## 📚 Epic: Blog Management System

### Epic Description
As a platform user, I want a complete blog management system that allows me to create, edit, publish, and view blog posts with images, so that I can share my ideas and read content from other users.

### Business Value
- Increases user engagement and content creation
- Provides a platform for knowledge sharing
- Enhances the application's value proposition
- Supports community building

---

## 📖 User Stories

### **User Story 1: Create Blog Post**

**Priority**: High (P0)  
**Story Points**: 8  
**Sprint**: Sprint 1

#### Story
```
As a registered user
I want to create blog posts with images and content
So that I can share my ideas and knowledge with others
```

#### Acceptance Criteria
- ✓ User can access "Create Blog" button from the Blogs page or navigation menu
- ✓ Create blog form includes the following fields:
  - Title (text input, required)
  - Content (textarea/rich text editor, required)
  - Featured Image (file upload, required)
  - Category (dropdown, optional)
  - Tags (multi-select/input, optional)
- ✓ Title validation:
  - Required field with error message
  - Maximum 100 characters
  - Minimum 5 characters
- ✓ Content validation:
  - Required field with error message
  - Minimum 50 characters
  - Maximum 10,000 characters
- ✓ Image upload validation:
  - Accepts only jpg, jpeg, png, gif formats
  - Maximum file size: 5MB
  - Display error message for invalid formats or size
  - Show image preview after selection
- ✓ Real-time character counter for title and content
- ✓ All validation errors display inline below respective fields
- ✓ Form includes "Preview" button to see blog before saving
- ✓ Form is responsive and works on mobile devices
- ✓ Required fields are marked with asterisk (*)

#### Technical Notes
- Use ASP.NET Core model validation attributes
- Implement IFormFile for image uploads
- Store images in wwwroot/uploads/blogs/ directory
- Generate unique filenames to prevent overwrites
- Use jQuery validation for client-side validation

#### Definition of Done
- [ ] Model created with proper validation attributes
- [ ] Controller action handles form submission
- [ ] View created with Bootstrap form styling
- [ ] Client-side validation implemented
- [ ] Server-side validation implemented
- [ ] Image upload functionality working
- [ ] Unit tests written for validation logic
- [ ] Integration tests for create blog endpoint
- [ ] Code reviewed and approved

---

### **User Story 2: Save as Draft or Publish**

**Priority**: High (P0)  
**Story Points**: 5  
**Sprint**: Sprint 1

#### Story
```
As a registered user
I want to save my blog as a draft or publish it immediately
So that I can work on it later or share it with everyone right away
```

#### Acceptance Criteria
- ✓ Create/Edit blog form has two action buttons:
  - "Save as Draft" (secondary button style)
  - "Publish" (primary button style)
- ✓ Draft functionality:
  - Saves blog with status = "Draft"
  - Draft blogs are only visible to the author
  - Draft blogs don't appear in public blog list
  - Can be edited and published later
- ✓ Publish functionality:
  - Saves blog with status = "Published"
  - Sets PublishedDate to current timestamp
  - Published blogs are visible to all authenticated users
  - Appears in main blog dashboard
- ✓ Success messages:
  - "Blog saved as draft successfully" for drafts
  - "Blog published successfully!" for published blogs
- ✓ After saving, user is redirected to:
  - "My Blogs" page showing the newly created blog
- ✓ Status indicator (badge) shows:
  - Yellow/Orange badge with "Draft" text
  - Green badge with "Published" text
- ✓ User can change status from Draft to Published when editing
- ✓ Cannot change from Published to Draft (optional: add unpublish feature)

#### Technical Notes
- Create BlogStatus enum (Draft = 0, Published = 1)
- Use different action methods or parameter to distinguish save actions
- Implement authorization to ensure draft visibility
- Use TempData for success messages
- Add PublishedDate (nullable datetime) field

#### Definition of Done
- [ ] BlogStatus enum created
- [ ] Draft/Publish logic implemented in controller
- [ ] Authorization rules applied for draft visibility
- [ ] Status badges displayed correctly in UI
- [ ] Success messages working with TempData
- [ ] Unit tests for draft/publish logic
- [ ] Integration tests for both workflows
- [ ] Code reviewed and approved

---

### **User Story 3: View Blogs on Main Site**

**Priority**: High (P0)  
**Story Points**: 8  
**Sprint**: Sprint 1

#### Story
```
As any site visitor
I want to view all published blogs on a dedicated Blogs page
So that I can discover and read content from users
```

#### Acceptance Criteria
- ✓ "Blogs" menu item added to main navigation (visible to all users)
- ✓ Clicking "Blogs" navigates to dedicated blogs page (/Blogs or /Blogs/Index)
- ✓ Blogs page displays all published blogs (not drafts)
- ✓ Blogs displayed in card-based grid layout:
  - 3 columns on desktop (≥992px)
  - 2 columns on tablet (768px - 991px)
  - 1 column on mobile (<768px)
- ✓ Each blog card displays:
  - Featured image (responsive, aspect ratio maintained)
  - Blog title (bold, truncated if too long)
  - Excerpt (first 150 characters of content with "...")
  - Author name (linked to author profile - future)
  - Category badge (if assigned)
  - Publication date (formatted: "MMM DD, YYYY")
  - Last updated date/time (formatted: "Last updated: MMM DD, YYYY HH:mm AM/PM")
  - View count (icon with number)
  - "Read More" button
- ✓ Blog cards have hover effect (slight shadow/scale)
- ✓ Blogs sorted by most recent first (descending by PublishedDate)
- ✓ Pagination implemented:
  - 9 blogs per page
  - Page numbers displayed at bottom
  - Previous/Next navigation
  - Current page highlighted
- ✓ Empty state:
  - "No blogs available yet. Be the first to create one!" message
  - "Create Blog" button visible (if authenticated)
- ✓ Loading indicator while fetching blogs
- ✓ Page title: "All Blogs" or "Blog Posts"
- ✓ Search bar at top (implementation in future story)
- ✓ Blogs accessible to both authenticated and unauthenticated users (public access)

#### Technical Notes
- Create BlogListViewModel for efficient data transfer
- Use LINQ Skip/Take for pagination
- Generate excerpt in controller or use stored excerpt field
- Implement responsive CSS Grid or Bootstrap cards
- Optimize query with pagination to avoid loading all blogs
- Consider caching for better performance

#### Definition of Done
- [ ] "Blogs" menu item added to navigation
- [ ] Blogs page (Index view) created with card layout
- [ ] Public access enabled (no authorization required)
- [ ] Pagination logic implemented
- [ ] Responsive design working on all devices
- [ ] All blog metadata displaying correctly
- [ ] Empty state UI implemented
- [ ] Loading states handled
- [ ] Performance optimized (N+1 query prevention)
- [ ] Unit tests for pagination logic
- [ ] Integration tests for blogs page endpoint
- [ ] Code reviewed and approved

---

### **User Story 4: Manage My Blogs**

**Priority**: High (P0)  
**Story Points**: 8  
**Sprint**: Sprint 2

#### Story
```
As a registered user
I want to view, edit, and delete my own blogs
So that I can manage my content effectively
```

#### Acceptance Criteria
- ✓ "My Blogs" link available in navigation menu
- ✓ My Blogs page shows only current user's blogs (both draft and published)
- ✓ Filter tabs at top:
  - "All" (default)
  - "Published"
  - "Drafts"
  - Count badge showing number of blogs in each category
- ✓ Each blog row/card displays:
  - Featured image thumbnail
  - Title
  - Status badge (Draft/Published)
  - Created date
  - Last updated date/time
  - View count (for published blogs)
  - Action buttons: Edit, Delete
- ✓ Edit functionality:
  - Opens same create form pre-filled with existing data
  - Page title: "Edit Blog"
  - Can modify all fields including status
  - Save button updates existing blog
  - LastUpdatedDate automatically updated
  - Success message: "Blog updated successfully"
- ✓ Delete functionality:
  - Confirmation dialog: "Are you sure you want to delete this blog? This action cannot be undone."
  - Confirm/Cancel buttons in dialog
  - On confirm: Blog is deleted (soft delete or hard delete)
  - Success message: "Blog deleted successfully"
  - Removed from list without page refresh (optional)
- ✓ Authorization:
  - Only blog author can edit/delete their own blogs
  - Attempting to edit others' blogs shows 403 Forbidden
- ✓ Sort options:
  - Most recent first (default)
  - Oldest first
  - Alphabetical by title
- ✓ Empty state: "You haven't created any blogs yet. Start writing!"

#### Technical Notes
- Filter by UserId in LINQ query
- Implement soft delete (IsDeleted flag) to preserve data
- Use authorization policies or custom checks
- JavaScript confirmation dialog or Bootstrap modal
- Update timestamp automatically using OnModelCreating
- Return appropriate HTTP status codes (403, 404)

#### Definition of Done
- [ ] My Blogs page created with filter tabs
- [ ] Edit functionality working with pre-filled form
- [ ] Delete with confirmation implemented
- [ ] Authorization checks in place
- [ ] Timestamp updates automatically
- [ ] Sort functionality working
- [ ] Empty state UI implemented
- [ ] Unit tests for edit/delete logic
- [ ] Integration tests for all operations
- [ ] Security testing for authorization
- [ ] Code reviewed and approved

---

### **User Story 5: View Full Blog Post**

**Priority**: High (P0)  
**Story Points**: 5  
**Sprint**: Sprint 1

#### Story
```
As any authenticated user
I want to view the complete blog post with all details
So that I can read the full content
```

#### Acceptance Criteria
- ✓ Clicking "Read More" navigates to blog details page
- ✓ URL format: /Blogs/Details/{id} or /blog/{slug}
- ✓ Full blog page displays:
  - Blog title (h1 heading)
  - Featured image (full width, responsive)
  - Full content (formatted with proper line breaks and paragraphs)
  - Author information:
    - Author name
    - Author role (badge)
    - "Posted by {author}" text
  - Published date (formatted: "Published on MMM DD, YYYY")
  - Last updated date (if different from published: "Last updated on MMM DD, YYYY at HH:mm AM/PM")
  - View count (icon with number)
  - Category badge (if assigned)
  - Tags (if assigned, clickable - future)
- ✓ View counter increments by 1 on each unique visit (per session)
- ✓ Navigation elements:
  - "← Back to Blogs" button/link
  - Breadcrumb: Home > Blogs > Blog Title
- ✓ Related blogs section (optional for MVP):
  - Shows 3 blogs from same category
  - Or 3 most recent blogs if no category match
  - Card format similar to blogs page
- ✓ Edit/Delete buttons visible only if:
  - Current user is the author, OR
  - Current user is Admin
- ✓ Social share buttons (optional):
  - Facebook, Twitter, LinkedIn, Copy Link
- ✓ Responsive layout works on all devices
- ✓ Images in content are responsive (max-width: 100%)
- ✓ 404 error page if blog not found
- ✓ 403 error if trying to view draft blog by non-author

#### Technical Notes
- Create BlogDetailsViewModel with all related data
- Increment ViewCount in controller action
- Use session to track unique views (prevent multiple counts)
- Generate slug from title for SEO-friendly URLs
- Sanitize HTML content to prevent XSS attacks
- Lazy load images for better performance
- Use eager loading for author information

#### Definition of Done
- [ ] Blog details page created
- [ ] View counter implemented with session tracking
- [ ] Author information displayed
- [ ] Edit/Delete buttons with proper authorization
- [ ] Related blogs section implemented
- [ ] Responsive design verified
- [ ] 404/403 error handling
- [ ] XSS prevention measures in place
- [ ] Unit tests for view counter logic
- [ ] Integration tests for details page
- [ ] Performance testing with large content
- [ ] Code reviewed and approved

---

### **User Story 6: Search and Filter Blogs**

**Priority**: Medium (P1)  
**Story Points**: 8  
**Sprint**: Sprint 2

#### Story
```
As a registered user
I want to search and filter blogs by various criteria
So that I can quickly find relevant content
```

#### Acceptance Criteria
- ✓ Search bar prominently placed at top of Blogs page
- ✓ Search functionality:
  - Searches in blog title (case-insensitive)
  - Searches in blog content (case-insensitive)
  - Searches in author name
  - Real-time search (updates as user types - debounced 500ms)
  - Placeholder text: "Search blogs by title, content, or author..."
  - Clear search button (X icon) appears when text entered
- ✓ Filter options (sidebar or dropdown):
  - Category dropdown (All Categories, Category 1, Category 2, etc.)
  - Author dropdown (All Authors, Author 1, Author 2, etc.)
  - Date range picker:
    - From date
    - To date
    - Preset options: Today, Last 7 Days, Last 30 Days, All Time
  - Tags multi-select (if tags implemented)
- ✓ Sort options:
  - Most Recent (default)
  - Oldest First
  - Most Viewed
  - Alphabetical (A-Z)
  - Alphabetical (Z-A)
- ✓ Filter behavior:
  - Filters are additive (AND logic)
  - URL parameters update with filters for bookmarking
  - Results update without full page refresh (AJAX)
  - Result count shown: "Showing X results"
  - No results message: "No blogs found matching your criteria"
- ✓ "Clear All Filters" button:
  - Resets all filters and search
  - Returns to default view
  - Only visible when filters are active
- ✓ Active filters displayed as removable badges/chips
- ✓ Filter state persists during session
- ✓ Loading indicator during search/filter operations

#### Technical Notes
- Implement dynamic LINQ queries for flexible filtering
- Use jQuery for AJAX search/filter
- Debounce search input to reduce server calls
- Return JSON for AJAX requests
- Update URL parameters for SEO and bookmarking
- Consider implementing ElasticSearch for advanced search (future)
- Cache filter options (categories, authors) for performance

#### Definition of Done
- [ ] Search functionality implemented with debouncing
- [ ] All filter options working
- [ ] Sort options implemented
- [ ] AJAX updates working smoothly
- [ ] URL parameter updates
- [ ] Clear filters functionality
- [ ] Active filters display
- [ ] No results state handled
- [ ] Loading states implemented
- [ ] Unit tests for search/filter logic
- [ ] Integration tests for all filter combinations
- [ ] Performance testing with large datasets
- [ ] Code reviewed and approved

---


#### Technical Notes
- Use IFormFile for file uploads
- Implement IImageService for image processing
- Use System.Drawing or ImageSharp library for resizing
- Store only filename in database, not full path
- Implement file size validation on server-side
- Use async file operations for better performance
- Consider cloud storage (Azure Blob, AWS S3) for production

#### Definition of Done
- [ ] Image upload field implemented
- [ ] File validation working (format and size)
- [ ] Preview functionality working
- [ ] Image processing service created
- [ ] Thumbnail generation working
- [ ] Unique filename generation
- [ ] Edit image replacement working
- [ ] Image deletion on blog delete
- [ ] Error handling implemented
- [ ] Unit tests for image service
- [ ] Integration tests for upload
- [ ] Manual testing on different devices
- [ ] Code reviewed and approved

---

## 🧭 Navigation Structure

### Main Navigation Menu
The application navigation will be updated to include a dedicated "Blogs" menu item:

```
Navigation Bar:
├── LOGINDEMO (Brand/Home)
├── Home (Dashboard - /Users/Index) - [Requires Authentication]
├── Blogs (/Blogs/Index) - [Public Access] ⭐ NEW
├── Login (/Users/Login) - [Guest Only]
├── Signup (/Users/Register) - [Guest Only]
└── Logout (/Users/Logout) - [Authenticated Users Only]
```

### Additional Navigation for Authenticated Users
When a user is logged in, additional blog-related options appear:

```
Authenticated User Navigation:
├── Blogs (View all public blogs)
├── My Blogs (/Blogs/MyBlogs) - View/manage own blogs ⭐ NEW
└── Create Blog (/Blogs/Create) - Create new blog post ⭐ NEW
```

### Admin-Specific Navigation
Admin users will see additional menu items:

```
Admin Navigation:
├── Blog Management (/Blogs/Admin) - Moderate all blogs ⭐ NEW
└── All other admin features
```

### Page Hierarchy
```
Main Site
├── Home/Dashboard (Users/Index)
│   └── User management and welcome page
├── Blogs (Blogs/Index) ⭐ NEW
│   ├── All published blogs (public access)
│   ├── Search and filter functionality
│   └── Pagination
├── Blog Details (Blogs/Details/{id})
│   └── Full blog post view
├── My Blogs (Blogs/MyBlogs) - Authenticated
│   ├── User's own blogs (draft + published)
│   └── Edit/Delete actions
├── Create Blog (Blogs/Create) - Authenticated
│   └── Blog creation form
└── Blog Management (Blogs/Admin) - Admin Only
    └── Moderate all blogs system-wide
```

### Access Control Summary
| Page | Access Level | Authorization |
|------|-------------|---------------|
| Blogs (Index) | **Public** | No authentication required |
| Blog Details | **Public** | No authentication required (published only) |
| Create Blog | **Authenticated** | Requires login |
| My Blogs | **Authenticated** | User can only view own blogs |
| Edit Blog | **Authenticated** | User can only edit own blogs |
| Delete Blog | **Authenticated** | User can only delete own blogs |
| Blog Management | **Admin Only** | Requires Admin role |

---

## 🗄️ Technical Architecture

### Database Schema

#### **Blog Table**
```sql
CREATE TABLE Blogs (
    BlogId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    Excerpt NVARCHAR(250),
    FeaturedImage NVARCHAR(255) NOT NULL,
    Slug NVARCHAR(150) UNIQUE NOT NULL,
    AuthorId NVARCHAR(128) NOT NULL,
    CategoryId INT NULL,
    Status INT NOT NULL DEFAULT 0, -- 0=Draft, 1=Published
    ViewCount INT NOT NULL DEFAULT 0,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    PublishedDate DATETIME2 NULL,
    LastUpdatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsDeleted BIT NOT NULL DEFAULT 0,
    FOREIGN KEY (AuthorId) REFERENCES Users(UserId),
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
)
```

#### **Category Table**
```sql
CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255),
    Slug NVARCHAR(60) UNIQUE NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
)
```

#### **Tag Table**
```sql
CREATE TABLE Tags (
    TagId INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(30) NOT NULL UNIQUE,
    Slug NVARCHAR(40) UNIQUE NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE()
)
```

#### **BlogTag Table (Many-to-Many)**
```sql
CREATE TABLE BlogTags (
    BlogId INT NOT NULL,
    TagId INT NOT NULL,
    PRIMARY KEY (BlogId, TagId),
    FOREIGN KEY (BlogId) REFERENCES Blogs(BlogId) ON DELETE CASCADE,
    FOREIGN KEY (TagId) REFERENCES Tags(TagId) ON DELETE CASCADE
)
```

#### **BlogAuditLog Table (Optional)**
```sql
CREATE TABLE BlogAuditLogs (
    AuditId INT PRIMARY KEY IDENTITY(1,1),
    BlogId INT NOT NULL,
    Action NVARCHAR(50) NOT NULL, -- Created, Updated, Deleted, Published, Unpublished
    PerformedBy NVARCHAR(128) NOT NULL,
    PerformedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
    Details NVARCHAR(MAX),
    FOREIGN KEY (BlogId) REFERENCES Blogs(BlogId),
    FOREIGN KEY (PerformedBy) REFERENCES Users(UserId)
)
```

### Models

#### **Blog.cs**
```csharp
public class Blog
{
    public int BlogId { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, MinimumLength = 5)]
    public string Title { get; set; }
    
    [Required(ErrorMessage = "Content is required")]
    [MinLength(50, ErrorMessage = "Content must be at least 50 characters")]
    public string Content { get; set; }
    
    [StringLength(250)]
    public string? Excerpt { get; set; }
    
    [Required]
    public string FeaturedImage { get; set; }
    
    [Required]
    public string Slug { get; set; }
    
    [Required]
    public string AuthorId { get; set; }
    
    public int? CategoryId { get; set; }
    
    [Required]
    public BlogStatus Status { get; set; }
    
    public int ViewCount { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? PublishedDate { get; set; }
    
    public DateTime LastUpdatedDate { get; set; }
    
    public bool IsDeleted { get; set; }
    
    // Navigation Properties
    public User Author { get; set; }
    public Category? Category { get; set; }
    public ICollection<BlogTag> BlogTags { get; set; }
}

public enum BlogStatus
{
    Draft = 0,
    Published = 1
}
```

### Services

#### **IImageService.cs**
```csharp
public interface IImageService
{
    Task<string> SaveImageAsync(IFormFile image, string folder);
    Task<bool> DeleteImageAsync(string imagePath);
    Task<string> ResizeImageAsync(string imagePath, int width, int height);
    bool ValidateImage(IFormFile image);
}
```

#### **IBlogService.cs**
```csharp
public interface IBlogService
{
    Task<IEnumerable<Blog>> GetAllPublishedBlogsAsync();
    Task<Blog> GetBlogByIdAsync(int id);
    Task<Blog> GetBlogBySlugAsync(string slug);
    Task<IEnumerable<Blog>> GetBlogsByAuthorAsync(string authorId);
    Task<bool> CreateBlogAsync(Blog blog);
    Task<bool> UpdateBlogAsync(Blog blog);
    Task<bool> DeleteBlogAsync(int id);
    Task IncrementViewCountAsync(int id);
    string GenerateSlug(string title);
    string GenerateExcerpt(string content, int length = 150);
}
```

---

## 📅 Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Nov 13, 2024 | AI Assistant | Initial user stories created |
| 1.1 | Nov 13, 2024 | AI Assistant | Updated to reflect blogs on main site with dedicated menu instead of dashboard |

---

**Document Status**: ✅ Ready for Review  
**Next Review Date**: [To be determined]

---

*This document is a living document and will be updated as requirements evolve.*

