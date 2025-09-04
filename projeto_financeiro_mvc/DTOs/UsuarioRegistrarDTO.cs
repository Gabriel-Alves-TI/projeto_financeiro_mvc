using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.DTOs
{
    public class UsuarioRegistrarDTO
    {
        [Required(ErrorMessage = "Digite o Nome!")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Digite o Sobrenome!")]
        public string Sobrenome { get; set; }
        [Required(ErrorMessage = "Digite o E-mail!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite a Senha!")]
        public string Senha { get; set; }
        [Required(ErrorMessage = "Digite a Confirmação de Senha!"),
        Compare("Senha", ErrorMessage = "As senhas não estão iguais!")]
        public string ConfirmaSenha { get; set; }
    }
}