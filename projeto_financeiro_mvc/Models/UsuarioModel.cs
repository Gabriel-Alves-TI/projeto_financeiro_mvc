using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public byte[] SenhaHash { get; set; }
        public byte[] SenhaSalt { get; set; }

        public int? GrupoFamiliarId { get; set; }
        public GrupoFamiliarModel GrupoFamiliar { get; set; }
        public ICollection<ContaModel> Contas { get; set; }
        public ICollection<LancamentoModel> Lancamentos { get; set; }
    }
}