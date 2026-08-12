using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class AdminLoginModel : PageModel
{
    [BindProperty]
    public string Password { get; set; }
    
    public string ErrorMessage { get; set; }
    
    public const string AdminPassword = "asdfghjkl";
    
    public IActionResult OnPost()
    {
        if (Password == AdminPassword)
        {
            HttpContext.Session.SetString("IsAdmin", "true");
            return RedirectToPage("/Admin");
        }

        ErrorMessage = "Vale parool. Proovi uuesti!";
        return Page();
    }
}