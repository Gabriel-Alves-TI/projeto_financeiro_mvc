using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.Services.EmailService
{
    public class EmailService : IEmailInterface
    {
        private readonly EmailSettings _emailSettings;
        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task EnviarEmailAsync(string destinatario, string assunto, string mensagem)
        {
            using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.Port))
            {
                client.Credentials = new NetworkCredential(_emailSettings.RemetenteEmail, _emailSettings.RemetenteSenha);
                client.EnableSsl = true;

                Console.WriteLine($"Remetente: '{_emailSettings.RemetenteEmail}'");
                Console.WriteLine($"Destinatário: '{destinatario}'");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.RemetenteEmail, _emailSettings.Remetente),
                    Subject = assunto,
                    Body = mensagem,
                    IsBodyHtml = true,
                    Priority = MailPriority.High
                };

                mailMessage.To.Add(destinatario);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}