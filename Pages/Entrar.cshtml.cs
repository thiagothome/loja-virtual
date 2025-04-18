using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EntrarModel : PageModel
{
    [BindProperty]
    public required string Email { get; set; }

    [BindProperty]
    public required string Senha { get; set; }

    [BindProperty]
    public bool LembrarMe { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

                return RedirectToPage("/Index");
    }
}