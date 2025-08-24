using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.ViewModels
{
    public class MovimentosFinanceirosViewModel
    {
        public List<LancamentoModel> Lancamentos { get; set; }
        public List<TransferenciaModel> Transferencias { get; set; }
        public List<RecorrenteModel> Recorrentes { get; set; }
        public List<ContaModel> Contas { get; set; }

        //Dados do Gráfico
        public string[] DiasSemana { get; set; }
        public List<double> ReceitaSemanal { get; set; }
        public List<double> DespesaSemanal { get; set; }
    }
}