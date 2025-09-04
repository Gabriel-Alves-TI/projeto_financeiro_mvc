using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Services.SenhaService;

namespace projeto_financeiro_mvc.Services.LoginService
{
    public class LoginService : ILoginInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;
        public LoginService(AppDbContext context, ISenhaInterface senhaInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
        }

        public async Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioRegistrarDTO usuarioRegistrarDto)
        {
            var response = new ResponseModel<UsuarioModel>();
            try
            {
                if (VerificarSeEmailExiste(usuarioRegistrarDto))
                {
                    response.Mensagem = "E-mail já cadastrado!";
                    response.Status = false;
                    return response;
                }

                _senhaInterface.CriarSenhaHash(usuarioRegistrarDto.Senha, out byte[] senhaHash, out byte[] senhaSalt);

                var usuario = new UsuarioModel()
                {
                    Nome = usuarioRegistrarDto.Nome,
                    Sobrenome = usuarioRegistrarDto.Sobrenome,
                    Email = usuarioRegistrarDto.Email,
                    SenhaHash = senhaHash,
                    SenhaSalt = senhaSalt
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                response.Mensagem = "Usuário cadastrado com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        private bool VerificarSeEmailExiste(UsuarioRegistrarDTO usuarioRegistrarDto)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioRegistrarDto.Email);

            if (usuario == null)
            {
                return false;
            }

            return true;
        }
    }
}