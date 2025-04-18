using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class CadastrarModel : PageModel
{
    [BindProperty]
    public string Nome { get; set; }

    [BindProperty]
    public string Email { get; set; }

    [BindProperty]
    public string Senha { get; set; }

    [BindProperty]
    public string ConfirmarSenha { get; set; }

    public IActionResult OnPost()
    {
        if (Senha != ConfirmarSenha)
        {
            ModelState.AddModelError("ConfirmarSenha", "As senhas não coincidem");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

                return RedirectToPage("/Index");
    }
}