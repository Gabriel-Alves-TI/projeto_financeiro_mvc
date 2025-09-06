using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace projeto_financeiro_mvc.DTOs
{
    public class UsuarioLoginDTO
    {
        [Required(ErrorMessage = "Digite o E-mail!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite a Senha!")]
        public string Senha { get; set; }
    }
}