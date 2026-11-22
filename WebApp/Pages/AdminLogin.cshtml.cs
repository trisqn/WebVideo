using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class AdminLoginModel : PageModel
{
    private readonly IConfiguration _configuration;

    public AdminLoginModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public string Password { get; set; } = string.Empty;
    
    public string? ErrorMessage { get; set; }
    
    public IActionResult OnPost()
    {
        var configuredPassword = _configuration["ADMIN_PASSWORD"];

        if (!string.IsNullOrEmpty(configuredPassword) &&
            string.Equals(Password, configuredPassword, StringComparison.Ordinal))
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            return RedirectToPage("/Admin");
        }

        ErrorMessage = "Vale parool. Proovi uuesti!";
        return Page();
    }
}