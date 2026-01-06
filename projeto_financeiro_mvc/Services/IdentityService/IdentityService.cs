using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.Services.IdentityService
{
    public class IdentityService : IIdentityInterface
    {
        public IEnumerable<Claim> CreateClaims(UsuarioModel usuario)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),

                new Claim("Theme",
                    usuario.Preferences?.Theme.ToString() ?? Theme.Vapor.ToString())
            };
        }
    }
}