using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.ViewModels
{
    public class LancamentoViewModel
    {
        public LancamentoDTO Lancamento { get; set; }
        [ValidateNever]
        [JsonIgnore]
        public List<ContaModel> Contas { get; set; }
    };

    public class Lancamentos
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public TipoLancamento Tipo { get; set; }
        public double Valor { get; set; }
        public string Categoria { get; set; }
        public bool Pago { get; set; }
        public string Conta { get; set; }
        public int? ContaId { get; set; }
        public string? ContaDestino { get; set; }
        public bool? IsRecorrente { get; set; }
        public string Origem { get; set; } // Lancamento, Recorrente, Transferencia
    }

    public class ListLancamentosViewModel
    {
        public List<Lancamentos> Movimentos { get; set; } = [];
        public List<ContaModel> Contas { get; set; }

        // Filtros
        public string? Tipo { get; set; }
        public string? Categoria { get; set; }
        public string? Descricao { set; get; }
        public double? Valor { set; get;}
        public int? ContaId { get; set; }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
    }
}