using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace projeto_financeiro_mvc.Views.Usuario.Preferences
{
    public class Theme : PageModel
    {
        private readonly ILogger<Theme> _logger;

        public Theme(ILogger<Theme> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}