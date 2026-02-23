using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Shared
{
    public class _ImportarLancamentosOfx : PageModel
    {
        private readonly ILogger<_ImportarLancamentosOfx> _logger;

        public _ImportarLancamentosOfx(ILogger<_ImportarLancamentosOfx> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}