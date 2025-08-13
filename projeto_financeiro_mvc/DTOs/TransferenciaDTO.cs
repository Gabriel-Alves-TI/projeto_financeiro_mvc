using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.DTOs
{
    public class TransferenciaDTO
    {
        public int? Id { get; set; }
        public string? Descricao { get; set; }
        public double Valor { get; set; }
        public string Categoria { get; set; }
        public TipoLancamento Tipo { get; set; }
        public DateTime DataTransferencia { get; set; }
        public DateTime DataCompensacao { get; set; }
        public int ContaOrigemId { get; set; }
        public int ContaDestinoId { get; set; }
    }
}