using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public string Email { get; set; }
        public byte[] SenhaHash { get; set; }
        public byte[] SenhaSalt { get; set; }
        public Preferences? Preferences { get; set;} //novo campo tblUsuario
        
        // Redefinição de Senha
        public string? Token { get; set; }
        public DateTime? ExpiracaoToken { get; set; }

        public int? GrupoFamiliarId
        { get; set; }
        public GrupoFamiliarModel GrupoFamiliar { get; set; }
        public ICollection<ContaModel> Contas { get; set; }
        public ICollection<LancamentoModel> Lancamentos { get; set; }
    }
}