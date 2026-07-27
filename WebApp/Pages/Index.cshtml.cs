using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

[IgnoreAntiforgeryToken]
[RequestSizeLimit(long.MaxValue)]
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

    public async Task<IActionResult> OnPostUploadAsync(List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest(new { message = "Choose at least one photo or video." });
        }

        var uploadPath = Path.Combine(_environment.ContentRootPath, "uploads");
        Directory.CreateDirectory(uploadPath);

        var savedCount = 0;

        foreach (var file in files.Where(file => file.Length > 0))
        {
            if (!IsPhotoOrVideo(file))
            {
                return BadRequest(new { message = $"{file.FileName} is not a photo or video." });
            }

            var fileName = BuildStoredFileName(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            savedCount++;
        }

        if (savedCount == 0)
        {
            return BadRequest(new { message = "The selected file was empty." });
        }

        return new JsonResult(new { message = $"Uploaded {savedCount} file(s)." });
    }

    private static bool IsPhotoOrVideo(IFormFile file)
    {
        return file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)
               || file.ContentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
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
