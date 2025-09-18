using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.DTOs
{
    public class RedefinicaoSenhaDTO
    {
        public string Token { get; set; }

        [Required(ErrorMessage = "Digite a nova senha!")]
        public string NovaSenha { get; set; }
        
        [Required(ErrorMessage = "Digite a confirmação da nova senha!"),
        Compare("NovaSenha", ErrorMessage = "As senhas não estão iguais!")]
        public string ConfirmaNovaSenha { get; set; }
    }
}