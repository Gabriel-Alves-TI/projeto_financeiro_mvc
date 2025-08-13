using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.DTOs
{
    public class ContaDTO
    {
        public string Banco { get; set; }
        public int Agencia { get; set; }
        public int NumeroConta { get; set; }
        public double? SaldoInicial { get; set; }
    }
}