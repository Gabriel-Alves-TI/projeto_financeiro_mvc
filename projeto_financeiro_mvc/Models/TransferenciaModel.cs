using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class TransferenciaModel : MovimentoFinanceiroBase
    {
        public TipoLancamento Tipo { get; set; }
        public DateTime DataTransferencia { get; set; }
        public DateTime DataCompensacao { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
        [JsonIgnore]
        public ContaModel ContaOrigem { get; set; }
        [JsonIgnore]
        public ContaModel ContaDestino { get; set; }
    }
}