using LoginandRegisterMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginandRegisterMVC.Data;

public static class CategorySeeder
{
    public static async Task SeedCategoriesAsync(UserContext context)
    {
        // Check if categories already exist
        if (await context.Categories.AnyAsync())
        {
            return; // Categories already seeded
        }

        var categories = new List<Category>
        {
            new Category
            {
                Name = "Technology",
                Description = "Tech news, tutorials, and reviews",
                Slug = "technology",
                CreatedDate = DateTime.Now
            },
            new Category
            {
                Name = "Lifestyle",
                Description = "Life, health, and wellness",
                Slug = "lifestyle",
                CreatedDate = DateTime.Now
            },
            new Category
            {
                Name = "Business",
                Description = "Business trends and strategies",
                Slug = "business",
                CreatedDate = DateTime.Now
            },
            new Category
            {
                Name = "Travel",
                Description = "Travel tips and destinations",
                Slug = "travel",
                CreatedDate = DateTime.Now
            },
            new Category
            {
                Name = "Food",
                Description = "Recipes and food reviews",
                Slug = "food",
                CreatedDate = DateTime.Now
            },
            new Category
            {
                Name = "Education",
                Description = "Learning and development",
                Slug = "education",
                CreatedDate = DateTime.Now
            }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }
}


