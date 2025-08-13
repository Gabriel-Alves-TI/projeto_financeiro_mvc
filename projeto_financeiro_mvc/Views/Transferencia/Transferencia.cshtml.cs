using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Lancamento
{
    public class Transferencia : PageModel
    {
        private readonly ILogger<Transferencia> _logger;

        public Transferencia(ILogger<Transferencia> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}