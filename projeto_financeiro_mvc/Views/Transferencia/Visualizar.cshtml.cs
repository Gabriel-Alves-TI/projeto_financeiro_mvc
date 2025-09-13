using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Transferencia
{
    public class Visualizar : PageModel
    {
        private readonly ILogger<Visualizar> _logger;

        public Visualizar(ILogger<Visualizar> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}