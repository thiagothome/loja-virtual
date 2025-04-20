namespace SiteAspas.Services
{
    public interface IEmailService
    {
        Task EnviarEmailConfirmacaoAsync(string email, string nome, string token);
        Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml);
    }
}