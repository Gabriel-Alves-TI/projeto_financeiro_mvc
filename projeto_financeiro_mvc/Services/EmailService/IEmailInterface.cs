using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Services.EmailService
{
    public interface IEmailInterface
    {
        Task EnviarEmailAsync(string destinatario, string assunto, string mensagem);
    }
}