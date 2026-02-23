using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.ViewModels
{
    public class OfxViewModel
    {
        public int? Id { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public int? CategoriaId { get; set; }
        public TipoLancamento Tipo { get; set; }
        public DateTime Data { get; set; }
        public DateTime Previsao { get; set; }
        public int Parcelas { get; set; } = 1;
        public bool Pago { get; set; } = false;
        public int? ContaId { get; set; }
    }

    public class ListOfxViewModel
    {
        public List<OfxViewModel> LancamentosOfx { get; set; } = [];
        public List<ContaModel> Contas {get;set;}
        public List<CategoriaModel> Categorias {get;set;}
        public int? ContaId { get; set;}
    }
}