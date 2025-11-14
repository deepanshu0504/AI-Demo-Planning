using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace LoginandRegisterMVC.Services;

public class ImageService : IImageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<ImageService> _logger;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public ImageService(IWebHostEnvironment environment, ILogger<ImageService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public bool ValidateImage(IFormFile image, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (image == null || image.Length == 0)
        {
            errorMessage = "Please select an image file";
            return false;
        }

        // Check file size
        if (image.Length > MaxFileSize)
        {
            errorMessage = "File size must be less than 5MB";
            return false;
        }

        // Check file extension
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            errorMessage = "Only JPG, JPEG, PNG, and GIF formats are allowed";
            return false;
        }

        // Verify it's actually an image by trying to load it
        try
        {
            using var memoryStream = new MemoryStream();
            image.CopyTo(memoryStream);
            memoryStream.Position = 0;
            using var imageSharp = Image.Load(memoryStream);
            // If we got here, it's a valid image
        }
        catch (Exception)
        {
            errorMessage = "The file is not a valid image";
            return false;
        }

        return true;
    }

    public async Task<string> SaveImageAsync(IFormFile image, string folder)
    {
        try
        {
            // Generate unique filename
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Create full path
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder); // Ensure directory exists

            var filePath = Path.Combine(uploadsFolder, fileName);

            // Save original image
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            // Create thumbnail (200x200)
            await CreateThumbnailAsync(filePath, 200, 200);

            // Create medium version (800x600)
            await CreateMediumAsync(filePath, 800, 600);

            _logger.LogInformation($"Image saved successfully: {fileName}");
            return fileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image");
            throw;
        }
    }

    public async Task<bool> DeleteImageAsync(string imagePath)
    {
        try
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var fileName = Path.GetFileNameWithoutExtension(imagePath);
            var extension = Path.GetExtension(imagePath);
            var folder = Path.GetDirectoryName(imagePath) ?? "blogs";

            // Delete original
            var fullPath = Path.Combine(_environment.WebRootPath, "uploads", folder, $"{fileName}{extension}");
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // Delete thumbnail
            var thumbnailPath = Path.Combine(_environment.WebRootPath, "uploads", folder, $"{fileName}_thumb{extension}");
            if (File.Exists(thumbnailPath))
            {
                File.Delete(thumbnailPath);
            }

            // Delete medium
            var mediumPath = Path.Combine(_environment.WebRootPath, "uploads", folder, $"{fileName}_medium{extension}");
            if (File.Exists(mediumPath))
            {
                File.Delete(mediumPath);
            }

            _logger.LogInformation($"Image deleted successfully: {imagePath}");
            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting image: {imagePath}");
            return false;
        }
    }

    public async Task<string> ResizeImageAsync(string imagePath, int width, int height)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, "uploads", imagePath);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("Image file not found", fullPath);
            }

            using var image = await Image.LoadAsync(fullPath);
            
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max
            }));

            var fileName = Path.GetFileNameWithoutExtension(imagePath);
            var extension = Path.GetExtension(imagePath);
            var resizedFileName = $"{fileName}_{width}x{height}{extension}";
            var resizedPath = Path.Combine(Path.GetDirectoryName(fullPath)!, resizedFileName);

            await image.SaveAsync(resizedPath);

            _logger.LogInformation($"Image resized successfully: {resizedFileName}");
            return resizedFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error resizing image: {imagePath}");
            throw;
        }
    }

    private async Task CreateThumbnailAsync(string imagePath, int width, int height)
    {
        try
        {
            using var image = await Image.LoadAsync(imagePath);
            
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Crop
            }));

            var fileName = Path.GetFileNameWithoutExtension(imagePath);
            var extension = Path.GetExtension(imagePath);
            var thumbnailPath = Path.Combine(Path.GetDirectoryName(imagePath)!, $"{fileName}_thumb{extension}");

            await image.SaveAsync(thumbnailPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating thumbnail");
        }
    }

    private async Task CreateMediumAsync(string imagePath, int width, int height)
    {
        try
        {
            using var image = await Image.LoadAsync(imagePath);
            
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width, height),
                Mode = ResizeMode.Max
            }));

            var fileName = Path.GetFileNameWithoutExtension(imagePath);
            var extension = Path.GetExtension(imagePath);
            var mediumPath = Path.Combine(Path.GetDirectoryName(imagePath)!, $"{fileName}_medium{extension}");

            await image.SaveAsync(mediumPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating medium image");
        }
    }
}


