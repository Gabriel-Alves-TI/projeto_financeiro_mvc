using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.Services.LoginService
{
    public interface ILoginInterface
    {
        Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioRegistrarDTO usuarioRegistrarDto);
        Task<ResponseModel<UsuarioModel>> Login(UsuarioLoginDTO usuarioLoginDto);
        Task<ResponseModel<UsuarioModel>> SolicitarRecuperacaoSenha(RecuperacaoSenhaDTO solicitacaoDto);
        Task<ResponseModel<UsuarioModel>> RedefinicaoSenha(RedefinicaoSenhaDTO redefinicaoDto);
    }
}