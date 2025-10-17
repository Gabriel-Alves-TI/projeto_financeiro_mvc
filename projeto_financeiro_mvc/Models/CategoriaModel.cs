using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class CategoriaModel
    {
        public int Id { get; set;}
        public string Descricao { get; set; }

        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; }
        public int? GrupoFamiliarId { get; set; }
        public GrupoFamiliarModel GrupoFamiliar { get; set; }
    }
}