using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

[DisableRequestSizeLimit]
public class IndexModel : PageModel
{
    private readonly IWebHostEnvironment _environment;

    public IndexModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile>? files)
    {
        if (files is null || files.Count == 0)
        {
            return BadRequest(new { message = "Choose at least one photo or video." });
        }

        var validFiles = files.Where(file => file.Length > 0).ToList();
        if (validFiles.Count == 0)
        {
            return BadRequest(new { message = "Valitud fail oli tühi." });
        }

        var invalidFile = validFiles.FirstOrDefault(file => !IsPhotoOrVideo(file));
        if (invalidFile is not null)
        {
            return BadRequest(new { message = $"{invalidFile.FileName} ei ole pilt ega video." });
        }

        var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        var savedCount = 0;

        foreach (var file in validFiles)
        {
            var fileName = BuildStoredFileName(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            savedCount++;
        }

        if (savedCount == 0)
        {
            return BadRequest(new { message = "Valitud fail oli tühi." });
        }

        return new JsonResult(new { message = $"Üles laetud {savedCount} üksus(t)." });
    }

    private static bool IsPhotoOrVideo(IFormFile file)
    {
        var contentType = file.ContentType?.Trim();
        if (!string.IsNullOrEmpty(contentType))
        {
            if (contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) ||
                contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return extension is
            ".jpg" or ".jpeg" or ".jpe" or ".jfif" or ".png" or ".gif" or
            ".webp" or ".avif" or ".heic" or ".heif" or ".bmp" or
            ".tif" or ".tiff" or ".svg" or
            ".mp4" or ".m4v" or ".mov" or ".mkv" or ".avi" or
            ".webm" or ".mpeg" or ".mpg" or ".mpe" or ".3gp" or
            ".mts" or ".m2ts" or ".wmv" or ".flv" or ".ogv";
    }

    private static string BuildStoredFileName(string originalFileName)
    {
        var safeName = Path.GetFileName(originalFileName);
        foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
        {
            safeName = safeName.Replace(invalidCharacter, '_');
        }

        if (string.IsNullOrWhiteSpace(safeName))
        {
            safeName = "upload";
        }

        return $"{DateTimeOffset.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}-{safeName}";
    }
}
