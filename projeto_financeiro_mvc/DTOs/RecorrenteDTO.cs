using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.DTOs
{
    public class RecorrenteDTO
    {
        public int? Id { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public string Categoria { get; set; }
        public TipoLancamento Tipo { get; set; }
        public DateTime Data { get; set; }
        public DateTime Previsao { get; set; }
        public int Parcelas { get; set; }
        public bool Pago { get; set; }
        public bool IsRecorrente { get; set; }
        public int ContaId { get; set; }
    }
}