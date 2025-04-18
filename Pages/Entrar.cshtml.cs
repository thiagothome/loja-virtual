using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class EntrarModel : PageModel
{
    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Senha { get; set; }

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