using Microsoft.AspNetCore.Http;

namespace LoginandRegisterMVC.Services;

public interface IImageService
{
    /// <summary>
    /// Saves an uploaded image to the specified folder and returns the filename
    /// </summary>
    Task<string> SaveImageAsync(IFormFile image, string folder);

    /// <summary>
    /// Deletes an image file from the specified path
    /// </summary>
    Task<bool> DeleteImageAsync(string imagePath);

    /// <summary>
    /// Creates a resized version of an image
    /// </summary>
    Task<string> ResizeImageAsync(string imagePath, int width, int height);

    /// <summary>
    /// Validates an uploaded image file (format, size)
    /// </summary>
    bool ValidateImage(IFormFile image, out string errorMessage);
}


