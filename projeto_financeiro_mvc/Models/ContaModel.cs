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
        public int Agencia { get; set; }
        public int NumeroConta { get; set; }
        public double Saldo { get; set; }
        public ICollection<LancamentoModel> Lancamentos { get; set; }
    }
}