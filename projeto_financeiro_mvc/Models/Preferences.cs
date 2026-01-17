using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.Models
{
    public class Preferences
    {
        public Theme? Theme { get; set; } //novo campo tblUsuario
        public Preferences()
        {
            
        }
        public Preferences(Theme theme)
        {
            Theme = theme;
        }
    }
}