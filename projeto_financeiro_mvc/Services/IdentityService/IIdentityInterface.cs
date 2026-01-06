using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.Services.IdentityService
{
    public interface IIdentityInterface
    {
        IEnumerable<Claim> CreateClaims(UsuarioModel usuario);
    }
}