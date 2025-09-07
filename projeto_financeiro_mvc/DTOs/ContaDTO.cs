using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.DTOs
{
    public class ContaDTO
    {
        public int? Id {get;set;}
        [Required(ErrorMessage = "Digite o nome do Banco!")]
        public string Banco { get; set; }
        [Required(ErrorMessage = "Digite o número da Agência!")]
        public string Agencia { get; set; }
        [Required(ErrorMessage = "Digite o Número da Conta!")]
        public string NumeroConta { get; set; }
        public double? SaldoInicial { get; set; }
    }
}