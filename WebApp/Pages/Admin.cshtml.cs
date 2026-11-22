using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Admin : PageModel
{
    private readonly IWebHostEnvironment _environment;
    
    public List<FileInfo> Files { get; set; } = new();

    public Admin(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        if (!IsAuthenticated())
        {
            return RedirectToPage("/AdminLogin");
        }
        
        var uploadPath = Path.Combine(
            _environment.ContentRootPath,
            "uploads");

        if (Directory.Exists(uploadPath))
        {
            Files = Directory.GetFiles(uploadPath)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .ToList();
        }
        else
        {
            Files = new List<FileInfo>();
        }
        
        return Page();
    }

    public IActionResult OnPostLogout()
    {
        if (!IsAuthenticated())
        {
            return RedirectToPage("/AdminLogin");
        }

        HttpContext.Session.Remove("IsAdmin");
        return RedirectToPage("/AdminLogin");
    }
    
    public IActionResult OnPostDelete(string fileName)
    {
        if (!IsAuthenticated())
        {
            return RedirectToPage("/AdminLogin");
        }

        var path = ResolveUploadPath(fileName);

        if (path is not null && System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        
        return RedirectToPage();
    }

    public IActionResult OnPostDownload(string fileName)
    {
        if (!IsAuthenticated())
        {
            return RedirectToPage("/AdminLogin");
        }

        var path = ResolveUploadPath(fileName);

        if (path is null || !System.IO.File.Exists(path))
        {
            return NotFound();
        }

        return PhysicalFile(
            path,
            "application/octet-stream",
            fileName);
    }

    private bool IsAuthenticated()
    {
        return HttpContext.Session.GetString("IsAdmin") == "true";
    }

    private string? ResolveUploadPath(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) ||
            Path.GetFileName(fileName) != fileName)
        {
            return null;
        }

        var uploadRoot = Path.GetFullPath(
            Path.Combine(_environment.ContentRootPath, "uploads"));
        var fullPath = Path.GetFullPath(Path.Combine(uploadRoot, fileName));

        if (!fullPath.StartsWith(
                uploadRoot + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return fullPath;
    }
    
}
    
    