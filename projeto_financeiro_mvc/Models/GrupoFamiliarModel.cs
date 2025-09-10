using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class GrupoFamiliarModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }

        public ICollection<UsuarioModel> Usuarios { get; set; }
        public ICollection<LancamentoModel> Lancamentos { get; set; }
    }
}