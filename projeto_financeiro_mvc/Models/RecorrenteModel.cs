using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class RecorrenteModel : MovimentoFinanceiroBase
    {
        public TipoLancamento Tipo { get; set; }
        public DateTime Data { get; set; }
        public DateTime Previsao { get; set; }
        public int Parcelas { get; set; }
        public bool Pago { get; set; }
        public bool IsRecorrente { get; set; }
        public int? ContaId { get; set; }
        [JsonIgnore]
        public ContaModel Conta { get; set; }
    }
}