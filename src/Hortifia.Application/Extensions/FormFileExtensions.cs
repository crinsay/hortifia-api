
using Microsoft.AspNetCore.Http;

namespace Hortifia.Application.Extensions;

public static class FormFileExtensions
{
    public static bool IsImage(this IFormFile file)
    {
        var allowedImageExtensions = new[]
        { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".tiff", ".jiff", ".jfif", ".svg",
            ".eps", ".bmp", ".raw" };

        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedImageExtensions.Contains(fileExtension);
    }
}
