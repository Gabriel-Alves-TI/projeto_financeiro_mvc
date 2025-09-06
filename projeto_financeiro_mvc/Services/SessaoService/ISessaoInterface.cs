using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.Services.SessaoService
{
    public interface ISessaoInterface
    {
        UsuarioModel BuscarSessao();
        void CriarSessao(UsuarioModel usuarioModel);
        void RemoverSessao();
    }
}