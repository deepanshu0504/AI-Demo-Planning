using Microsoft.EntityFrameworkCore;
using LoginandRegisterMVC.Models;

namespace LoginandRegisterMVC.Data;

public class UserContext : DbContext
{
    public UserContext(DbContextOptions<UserContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Blog> Blogs { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<BlogTag> BlogTags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasMaxLength(128);
            entity.Property(e => e.Username).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.Role).IsRequired();
        });

        // Blog entity configuration
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Excerpt).HasMaxLength(250);
            entity.Property(e => e.FeaturedImage).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(150);
            entity.Property(e => e.AuthorId).IsRequired().HasMaxLength(128);
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.ViewCount).HasDefaultValue(0);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.LastUpdatedDate).HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);

            // Indexes for performance
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.PublishedDate);
            entity.HasIndex(e => e.AuthorId);
            entity.HasIndex(e => e.IsDeleted);

            // Relationships
            entity.HasOne(e => e.Author)
                .WithMany()
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Blogs)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Category entity configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(60);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // Tag entity configuration
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.TagId);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Slug).IsRequired().HasMaxLength(40);
            entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");

            entity.HasIndex(e => e.Name).IsUnique();
            entity.HasIndex(e => e.Slug).IsUnique();
        });

        // BlogTag entity configuration (Many-to-Many)
        modelBuilder.Entity<BlogTag>(entity =>
        {
            entity.HasKey(e => new { e.BlogId, e.TagId });

            entity.HasOne(e => e.Blog)
                .WithMany(b => b.BlogTags)
                .HasForeignKey(e => e.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tag)
                .WithMany(t => t.BlogTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
