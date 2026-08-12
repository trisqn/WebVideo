using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class Admin : PageModel
{
    private readonly IWebHostEnvironment _environment;
    
    public List<FileInfo> Files { get; set; }

    public Admin(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("IsAdmin") != "true")
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
        HttpContext.Session.Remove("IsAdmin");
        return RedirectToPage("/AdminLogin");
    }
    
    public IActionResult OnPostDelete(string fileName)
    {
        var path = Path.Combine(
            _environment.ContentRootPath,
            "uploads",
            fileName);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }
        
        return RedirectToPage();
    }

    public IActionResult OnPostDownload(string fileName)
    {
        var path = Path.Combine(
            _environment.ContentRootPath,
            "uploads",
            fileName);

        if (!System.IO.File.Exists(path))
        {
            return NotFound();
        }

        return PhysicalFile(
            path,
            "application/octet-stream",
            fileName);
    }
    
}
    
    