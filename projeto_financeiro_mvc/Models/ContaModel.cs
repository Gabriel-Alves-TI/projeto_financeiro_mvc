using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class ContaModel
    {
        public int Id { get; set; }
        public string Banco { get; set; }
        public string Agencia { get; set; }
        public string NumeroConta { get; set; }
        public double Saldo { get; set; }

        public int UsuarioId { get; set; }
        public UsuarioModel Usuario { get; set; }
        public int? GrupoFamiliarId { get; set; }
        public GrupoFamiliarModel GrupoFamiliar { get; set; }
        public ICollection<LancamentoModel> Lancamentos { get; set; }
    }
}