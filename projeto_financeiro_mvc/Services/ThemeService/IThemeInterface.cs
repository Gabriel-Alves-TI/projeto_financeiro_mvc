using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.Services.ThemeService
{
    public interface IThemeInterface
    {
        Theme GetCurrentTheme();
        Task<ResponseModel<Theme>> ChangeTheme(int usuarioId, Theme theme);
    }
}