using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Shared
{
    public class _ModalImportarOfx : PageModel
    {
        private readonly ILogger<_ModalImportarOfx> _logger;

        public _ModalImportarOfx(ILogger<_ModalImportarOfx> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}