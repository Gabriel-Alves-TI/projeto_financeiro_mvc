using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Conta
{
    public class ListarContas : PageModel
    {
        private readonly ILogger<ListarContas> _logger;

        public ListarContas(ILogger<ListarContas> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}