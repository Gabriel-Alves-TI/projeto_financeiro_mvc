using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Login
{
    public class RecuperarSenha : PageModel
    {
        private readonly ILogger<RecuperarSenha> _logger;

        public RecuperarSenha(ILogger<RecuperarSenha> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}