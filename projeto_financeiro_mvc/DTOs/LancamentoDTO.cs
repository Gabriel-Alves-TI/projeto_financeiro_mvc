using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.DTOs
{
    public class LancamentoDTO
    {
        public int? Id { get; set; }
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public int? CategoriaId { get; set; }
        public TipoLancamento Tipo { get; set; }
        public DateTime Data { get; set; }
        public DateTime Previsao { get; set; }
        public int Parcelas { get; set; }
        public bool Pago { get; set; }
        public int? ContaId { get; set; }
    }
}