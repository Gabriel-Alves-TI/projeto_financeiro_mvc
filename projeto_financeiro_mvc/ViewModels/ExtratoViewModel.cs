using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.ViewModels
{
    public class ExtratoViewModel
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public string Tipo { get; set; }
        public double Valor { get; set; }
        public string Categoria { get; set; }
        public string Conta { get; set; }
        public string Origem { get; set; } // Lancamento, Recorrente, Transferencia
    };

    public class ListExtratoViewModel
    {
        public List<ExtratoViewModel> Movimentos { get; set; } = [];
    }
}