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
        public string Descricao { get; set; }
        public double Valor { get; set; }
        public string Categoria { get; set; }
        public string Tipo { get; set; }
        public DateTime Data => DateTime.Now;
        public DateTime Previsao => DateTime.Now;
        public int Parcelas { get; set; }
        public bool Pago { get; set; }
        public bool Recorrente { get; set; }
        public int ContaId { get; set; }
    }
}