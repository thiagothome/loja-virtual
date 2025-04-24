using System.Net;
using System.Net.Mail;

namespace SiteAspas.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarEmailConfirmacaoAsync(string userId, string email, string nome, string token)
        {
            var appUrl = _configuration["AppUrl"]?.TrimEnd('/');

            if (string.IsNullOrEmpty(appUrl))
            {
                throw new ArgumentException("AppUrl não configurada");
            }

            string assunto = "Confirme seu cadastro";
            string linkConfirmacao = $"{appUrl}/ConfirmarEmail?userId={userId}&code={WebUtility.UrlEncode(token)}";

            string corpoHtml = $@"
                <html>
                    <body>
                        <h2>Olá, {nome}!</h2>
                        <p>Clique no link abaixo para confirmar seu e-mail:</p>
                        <p><a href='{linkConfirmacao}' style='color: #1a73e8;'>Confirmar E-mail</a></p>
                        <p>Se você não solicitou este cadastro, ignore este e-mail.</p>
                        <p><small>Link expira em 24 horas.</small></p>
                    </body>
                </html>";

            await EnviarEmailAsync(email, assunto, corpoHtml);
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string corpoHtml)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            using (var client = new SmtpClient(smtpSettings["Host"]))
            {
                client.Port = int.Parse(smtpSettings["Port"]);
                client.Credentials = new NetworkCredential(
                    smtpSettings["Username"],
                    smtpSettings["Password"]
                );
                client.EnableSsl = bool.Parse(smtpSettings["EnableSsl"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings["FromEmail"], smtpSettings["FromName"]),
                    Subject = assunto,
                    Body = corpoHtml,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(destinatario);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}