using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.ViewModels
{
    public class UsuarioViewModel
    {
        public int UsuarioId { get; set;}
        public Theme? Theme { get; set;}
        public IEnumerable<Theme> ThemesDisponiveis { get; set;}
    }
}