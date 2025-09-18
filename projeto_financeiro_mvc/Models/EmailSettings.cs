using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Remetente { get; set; } = string.Empty;
        public string RemetenteEmail { get; set; } = string.Empty;
        public string RemetenteSenha { get; set; } = string.Empty;
        
    }
}