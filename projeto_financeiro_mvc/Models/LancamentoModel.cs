using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.Models
{
    public class LancamentoModel
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public DateTime Data { get; set; }
        public DateTime Previsao { get; set; }
        public int Parcelas { get; set; }
        public bool Pago { get; set; }
        public bool Fixo { get; set; }
        public int ContaId { get; set; }
        [JsonIgnore]
        public ContaModel Conta { get; set; }
    }
}