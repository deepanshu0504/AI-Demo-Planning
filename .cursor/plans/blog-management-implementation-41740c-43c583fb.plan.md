<!-- 43c583fb-5711-481a-8795-9d657d0798c1 68ed6429-f77c-4df2-ba28-9110bd1a494f -->
# Blog Management System Implementation Plan

## Overview

Implement a comprehensive blog management system for the existing ASP.NET Core MVC 8 application, enabling users to create, manage, and view blog posts with images, draft/publish workflows, and search capabilities.

---

## Phase 1: Foundation & Database Setup

### 1.1 Database Models & Migrations

**Files to Create:**

- `src/LoginandRegisterMVC/Models/Blog.cs`
- `src/LoginandRegisterMVC/Models/Category.cs`
- `src/LoginandRegisterMVC/Models/Tag.cs`
- `src/LoginandRegisterMVC/Models/BlogTag.cs`
- `src/LoginandRegisterMVC/Models/BlogStatus.cs` (enum)

**Implementation Steps:**

1. Create Blog model with validation attributes:

   - Properties: BlogId, Title (5-100 chars), Content (50-10000 chars), Excerpt, FeaturedImage, Slug, AuthorId, CategoryId, Status, ViewCount, CreatedDate, PublishedDate, LastUpdatedDate, IsDeleted
   - Navigation properties: Author (User), Category, BlogTags

2. Create Category model:

   - Properties: CategoryId, Name, Description, Slug, CreatedDate

3. Create Tag model:

   - Properties: TagId, Name, Slug, CreatedDate

4. Create BlogTag junction model for many-to-many relationship

5. Create BlogStatus enum (Draft = 0, Published = 1)

**Files to Modify:**

- `src/LoginandRegisterMVC/Data/UserContext.cs`
  - Add DbSet<Blog> Blogs
  - Add DbSet<Category> Categories
  - Add DbSet<Tag> Tags
  - Add DbSet<BlogTag> BlogTags
  - Configure entity relationships in OnModelCreating
  - Set up cascade delete for BlogTags
  - Configure default values and constraints

**Migration:**

- Run: `dotnet ef migrations add AddBlogManagementTables`
- Run: `dotnet ef database update`

---

## Phase 2: Service Layer Implementation

### 2.1 Image Service

**Files to Create:**

- `src/LoginandRegisterMVC/Services/IImageService.cs`
- `src/LoginandRegisterMVC/Services/ImageService.cs`

**Implementation:**

- SaveImageAsync: Save image with GUID filename to wwwroot/uploads/blogs/
- DeleteImageAsync: Remove image file from disk
- ResizeImageAsync: Create thumbnail (200x200) and medium (800x600) versions
- ValidateImage: Check file type (jpg, jpeg, png, gif) and size (max 5MB)
- Use ImageSharp library for image processing

**Directory to Create:**

- `src/LoginandRegisterMVC/wwwroot/uploads/blogs/`

### 2.2 Blog Service

**Files to Create:**

- `src/LoginandRegisterMVC/Services/IBlogService.cs`
- `src/LoginandRegisterMVC/Services/BlogService.cs`

**Implementation:**

- GetAllPublishedBlogsAsync: Retrieve published blogs with pagination
- GetBlogByIdAsync: Get single blog with author and category
- GetBlogBySlugAsync: Get blog by SEO-friendly slug
- GetBlogsByAuthorAsync: Get all blogs for specific author
- CreateBlogAsync: Create new blog with slug generation
- UpdateBlogAsync: Update existing blog with timestamp
- DeleteBlogAsync: Soft delete (set IsDeleted = true)
- IncrementViewCountAsync: Increment view counter
- GenerateSlug: Convert title to URL-friendly slug
- GenerateExcerpt: Extract first 150 characters

**ViewModels to Create:**

- `src/LoginandRegisterMVC/Models/BlogListViewModel.cs`
- `src/LoginandRegisterMVC/Models/BlogDetailsViewModel.cs`
- `src/LoginandRegisterMVC/Models/CreateBlogViewModel.cs`
- `src/LoginandRegisterMVC/Models/MyBlogsViewModel.cs`

**Files to Modify:**

- `src/LoginandRegisterMVC/Program.cs`
  - Register IImageService and ImageService
  - Register IBlogService and BlogService
  - Add package: ImageSharp (via NuGet)

---

## Phase 3: User Story 1 & 2 - Create Blog with Draft/Publish

### 3.1 BlogsController - Create Actions

**Files to Create:**

- `src/LoginandRegisterMVC/Controllers/BlogsController.cs`

**Implementation:**

1. GET Create action [Authorize]:

   - Load categories for dropdown
   - Return CreateBlogViewModel

2. POST Create action [Authorize] [ValidateAntiForgeryToken]:

   - Validate model state
   - Validate image (format, size)
   - Save image using ImageService
   - Generate slug from title
   - Set AuthorId from current user
   - Set CreatedDate and LastUpdatedDate
   - Set Status (Draft or Published based on button clicked)
   - Set PublishedDate if publishing
   - Save to database
   - TempData success message
   - Redirect to MyBlogs

**Parameters for dual save:**

- Use two submit buttons with different names or single action with status parameter

### 3.2 Create Blog View

**Files to Create:**

- `src/LoginandRegisterMVC/Views/Blogs/Create.cshtml`

**Implementation:**

- Form with Bootstrap styling
- Title input (5-100 chars, required, character counter)
- Content textarea (50-10000 chars, required, character counter)
- Featured image upload with preview
- Category dropdown (optional)
- Two submit buttons: "Save as Draft" (btn-secondary) and "Publish" (btn-primary)
- Client-side validation with jQuery
- Image preview JavaScript
- Real-time character counters
- Required field indicators (*)

**JavaScript Features:**

- Image preview on file selection
- Character counters updating on keyup
- File validation (size, type)
- Form validation

---

## Phase 4: User Story 3 - Public Blogs Page

### 4.1 BlogsController - Index Action

**Implementation in BlogsController:**

- GET Index action [AllowAnonymous]:
  - Get published blogs only (Status = Published, IsDeleted = false)
  - Include Author and Category
  - Implement pagination (9 per page)
  - Order by PublishedDate descending
  - Return BlogListViewModel with pagination info

### 4.2 Blogs Index View

**Files to Create:**

- `src/LoginandRegisterMVC/Views/Blogs/Index.cshtml`

**Implementation:**

- Bootstrap card grid layout (col-lg-4, col-md-6, col-sm-12)
- Each card displays:
  - Featured image with alt text
  - Title (truncated if long)
  - Excerpt (150 chars)
  - Author name with icon
  - Category badge
  - Published date (format: "MMM DD, YYYY")
  - Last updated (if different)
  - View count with eye icon
  - "Read More" button
- Hover effects (transform: scale(1.02), box-shadow)
- Pagination controls at bottom
- Empty state: "No blogs available yet..."
- "Create Blog" button (visible only if authenticated)

**CSS Updates:**

- Add blog card styles to `src/LoginandRegisterMVC/wwwroot/css/Site.css`
- Responsive grid
- Card hover effects
- Image aspect ratio preservation

### 4.3 Navigation Update

**Files to Modify:**

- `src/LoginandRegisterMVC/Views/Shared/_Layout.cshtml`

**Changes:**

- Add "Blogs" menu item after "Home" (visible to all)
- Add "My Blogs" menu item (visible when authenticated)
- Add "Create Blog" menu item (visible when authenticated)
- Add "Blog Management" menu item (visible to Admin only)

---

## Phase 5: User Story 5 - View Full Blog Post

### 5.1 BlogsController - Details Action

**Implementation in BlogsController:**

- GET Details/{id} action [AllowAnonymous]:
  - Get blog by ID with Author and Category
  - Check if blog exists (404 if not)
  - Check authorization (403 if draft and not author/admin)
  - Increment view count (session-based unique tracking)
  - Get 3 related blogs (same category or recent)
  - Return BlogDetailsViewModel

### 5.2 Blog Details View

**Files to Create:**

- `src/LoginandRegisterMVC/Views/Blogs/Details.cshtml`

**Implementation:**

- Breadcrumb: Home > Blogs > [Title]
- Blog title (h1)
- Featured image (full width, responsive)
- Author info with role badge
- Published date and last updated
- View count with icon
- Category badge
- Full content (formatted, XSS-safe)
- Edit/Delete buttons (visible to author and admin only)
- "Back to Blogs" link
- Related blogs section (3 cards)
- Social share buttons (optional)

**Security:**

- Use @Html.Raw() with sanitized content or encode properly
- Implement XSS prevention

---

## Phase 6: User Story 4 - Manage My Blogs

### 6.1 BlogsController - MyBlogs, Edit, Delete Actions

**Implementation in BlogsController:**

1. GET MyBlogs action [Authorize]:

   - Get all blogs by current user (draft + published)
   - Filter by status if provided (all/published/drafts)
   - Order by LastUpdatedDate descending
   - Return MyBlogsViewModel with counts

2. GET Edit/{id} action [Authorize]:

   - Get blog by ID
   - Verify author (403 if not owner and not admin)
   - Load categories
   - Return Create view with pre-filled data

3. POST Edit/{id} action [Authorize] [ValidateAntiForgeryToken]:

   - Validate ownership
   - Update blog properties
   - Handle image replacement if new image uploaded
   - Delete old image if replaced
   - Update LastUpdatedDate
   - Update PublishedDate if status changed to Published
   - Save changes
   - TempData success message
   - Redirect to MyBlogs

4. POST Delete/{id} action [Authorize] [ValidateAntiForgeryToken]:

   - Validate ownership
   - Soft delete (set IsDeleted = true)
   - Delete associated images
   - TempData success message
   - Return JSON for AJAX or redirect

### 6.2 My Blogs View

**Files to Create:**

- `src/LoginandRegisterMVC/Views/Blogs/MyBlogs.cshtml`

**Implementation:**

- Filter tabs: All, Published (count), Drafts (count)
- Table or card layout with:
  - Thumbnail image
  - Title
  - Status badge (colored)
  - Created date
  - Last updated date/time
  - View count
  - Edit button (btn-primary)
  - Delete button (btn-danger)
- Sort dropdown (Recent, Oldest, A-Z)
- Empty state message
- JavaScript for delete confirmation
- Bootstrap modal or confirm dialog

**JavaScript:**

- Delete confirmation with modal
- Filter tab switching
- Sort functionality
- AJAX delete (optional)

---

## Phase 7: User Story 6 - Search and Filter

### 7.1 BlogsController - Search Action

**Implementation in BlogsController:**

- GET/POST Search action [AllowAnonymous]:
  - Accept search query, category filter, author filter, date range, sort option
  - Build dynamic LINQ query
  - Apply filters with AND logic
  - Return JSON for AJAX or partial view
  - Update URL parameters

### 7.2 Search UI Enhancement

**Files to Modify:**

- `src/LoginandRegisterMVC/Views/Blogs/Index.cshtml`

**Add:**

- Search bar at top (prominent)
- Filter sidebar or dropdown:
  - Category dropdown
  - Author dropdown  
  - Date range picker
  - Sort dropdown
- "Clear All Filters" button
- Active filter badges/chips
- Result count display
- Loading spinner
- No results message

**JavaScript:**

- Debounced search (500ms delay)
- AJAX filter updates
- URL parameter updates
- Filter badge removal
- Clear all filters

---

## Phase 8: Testing Implementation

### 8.1 Unit Tests

**Files to Create:**

- `tests/LoginandRegisterMVC.UnitTests/Services/ImageServiceTests.cs`
- `tests/LoginandRegisterMVC.UnitTests/Services/BlogServiceTests.cs`
- `tests/LoginandRegisterMVC.UnitTests/Controllers/BlogsControllerTests.cs`

**Test Coverage:**

- ImageService: Validate, Save, Delete, Resize
- BlogService: CRUD operations, slug generation, excerpt
- BlogsController: All actions with various scenarios
- Model validation tests

### 8.2 Integration Tests

**Files to Create:**

- `tests/LoginandRegisterMVC.IntegrationTests/Controllers/BlogsControllerIntegrationTests.cs`

**Test Scenarios:**

- Create blog workflow (draft and publish)
- View blogs (public access)
- Edit/delete authorization
- Search and filter functionality
- View counter increment
- Image upload integration

---

## Phase 9: UI/UX Polish

### 9.1 CSS Enhancements

**Files to Modify:**

- `src/LoginandRegisterMVC/wwwroot/css/Site.css`

**Add Styles:**

- Blog card animations
- Image hover effects
- Status badge colors (draft: #ffc107, published: #28a745)
- Form styling improvements
- Responsive breakpoints
- Loading spinners
- Button hover states

### 9.2 JavaScript Utilities

**Files to Create:**

- `src/LoginandRegisterMVC/wwwroot/js/blog.js`

**Functions:**

- Image preview
- Character counter
- Form validation
- Delete confirmation
- Search debounce
- Filter management
- AJAX handlers

---

## Phase 10: Documentation & Deployment

### 10.1 Update Documentation

**Files to Modify:**

- `README.md`

**Add:**

- Blog management feature description
- New database tables documentation
- API endpoints listing
- User guide for creating blogs
- Admin moderation guide

### 10.2 Database Seeding (Optional)

**Files to Create:**

- `src/LoginandRegisterMVC/Data/BlogSeeder.cs`

**Implementation:**

- Seed 5-10 sample categories
- Seed 10-20 sample blogs with images
- Assign to existing users
- Run on first startup

---

## Implementation Order & Dependencies

### Sprint 1 (MVP - 2 weeks):

1. Database models and migrations
2. Service layer (ImageService, BlogService)
3. Create blog functionality (User Stories 1 & 2)
4. Public blogs page (User Story 3)
5. Blog details page (User Story 5)
6. Navigation updates

### Sprint 2 (Management - 1.5 weeks):

7. My Blogs page (User Story 4)
8. Edit/Delete functionality
9. Search and filter (User Story 6)
10. Categories implementation

### Sprint 3 (Testing & Polish - 1 week):

11. Unit and integration tests
12. UI/UX refinements
13. Performance optimization
14. Documentation

---

## Key Technical Decisions

1. **Image Storage**: Local file system (wwwroot/uploads/blogs/) - can migrate to cloud storage later
2. **Soft Delete**: Use IsDeleted flag to preserve data
3. **Slug Generation**: Convert title to lowercase, replace spaces with hyphens
4. **View Counter**: Session-based tracking to prevent multiple counts
5. **Authorization**: Custom checks for blog ownership, role-based for admin
6. **Pagination**: Server-side with 9 blogs per page
7. **Search**: LINQ queries initially, can upgrade to ElasticSearch later
8. **Image Library**: ImageSharp for cross-platform compatibility

---

## Dependencies & Packages

Add to `src/LoginandRegisterMVC/LoginandRegisterMVC.csproj`:

```xml
<PackageReference Include="SixLabors.ImageSharp" Version="3.1.0" />
<PackageReference Include="SixLabors.ImageSharp.Web" Version="3.1.0" />
```

---

## Security Considerations

1. Validate all file uploads (type, size, content)
2. Sanitize HTML content before display
3. Use anti-forgery tokens on all POST actions
4. Implement authorization checks for edit/delete
5. Prevent path traversal in image filenames
6. Use parameterized queries (EF Core handles this)
7. Implement rate limiting for blog creation (future)

---

## Performance Optimizations

1. Eager loading for related entities (Include)
2. Pagination to avoid loading all blogs
3. Image optimization and thumbnail generation
4. Caching for category/author dropdowns
5. Index on Slug, Status, PublishedDate columns
6. Async/await throughout
7. Consider output caching for public blogs page

---

## Files Summary

**New Files (40+):**

- 5 Models (Blog, Category, Tag, BlogTag, BlogStatus)
- 4 ViewModels
- 2 Services with interfaces
- 1 Controller (BlogsController)
- 6 Views (Create, Index, Details, MyBlogs, Edit, partials)
- 2 JavaScript files
- 10+ Test files
- 1 Migration file

**Modified Files:**

- UserContext.cs
- Program.cs
- _Layout.cshtml
- Site.css
- README.md

This plan provides a comprehensive roadmap for implementing all 6 user stories with proper architecture, testing, and documentation.

### To-dos

- [ ] Create database models (Blog, Category, Tag, BlogTag, BlogStatus) and configure UserContext with relationships, constraints, and migrations
- [ ] Implement ImageService for image upload/resize/delete and BlogService for CRUD operations, slug generation, and excerpt creation
- [ ] Implement User Stories 1 & 2: Create blog functionality with draft/publish workflow, image upload, validation, and Create view
- [ ] Implement User Story 3: Public blogs page with card grid layout, pagination, and navigation menu updates
- [ ] Implement User Story 5: Blog details page with view counter, related blogs, author info, and edit/delete buttons for owners
- [ ] Implement User Story 4: My Blogs page with filter tabs, edit/delete functionality, authorization checks, and soft delete
- [ ] Implement User Story 6: Search and filter functionality with AJAX, debouncing, multiple filters, sort options, and URL parameters
- [ ] Write comprehensive unit tests for services and controllers, integration tests for all blog workflows, and authorization tests
- [ ] Add CSS animations, hover effects, loading states, JavaScript utilities, and ensure responsive design across all devices
- [ ] Update README with feature documentation, add sample data seeding, and create user/admin guides