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
    }
}