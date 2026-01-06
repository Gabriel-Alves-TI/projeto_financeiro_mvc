using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using projeto_financeiro_mvc.Models.Enums;
using projeto_financeiro_mvc.Services.ThemeService;

namespace projeto_financeiro_mvc.ViewComponents
{
    public class ThemeViewComponent : ViewComponent
    {
        private readonly IThemeInterface _themeInterface;

        public ThemeViewComponent(IThemeInterface themeInterface)
        {
            _themeInterface = themeInterface;
        }

        public IViewComponentResult Invoke()
        {
            var theme = _themeInterface.GetCurrentTheme();
            return View(theme);
        }
    }
}