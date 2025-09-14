using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Services.SenhaService;
using projeto_financeiro_mvc.Services.SessaoService;

namespace projeto_financeiro_mvc.Services.LoginService
{
    public class LoginService : ILoginInterface
    {
        private readonly AppDbContext _context;
        private readonly ISenhaInterface _senhaInterface;
        private readonly ISessaoInterface _sessaoInterface;
        public LoginService(AppDbContext context, ISenhaInterface senhaInterface, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _senhaInterface = senhaInterface;
            _sessaoInterface = sessaoInterface;
        }

        public async Task<ResponseModel<UsuarioModel>> Login(UsuarioLoginDTO usuarioLoginDto)
        {
            var response = new ResponseModel<UsuarioModel>();

            try
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == usuarioLoginDto.Email);
                if (usuario == null)
                {
                    response.Mensagem = "Credenciais Inválidas!";
                    response.Status = false;
                    return response;
                }

                if (!_senhaInterface.VerificaSenha(usuarioLoginDto.Senha, usuario.SenhaHash, usuario.SenhaSalt))
                {
                    response.Mensagem = "Credenciais Inválidas!";
                    response.Status = false;
                    return response;
                }

                //Criar uma sessão
                _sessaoInterface.CriarSessao(usuario);

                response.Mensagem = "Usuário logado com sucesso!";
                return response;
            }
            catch (Exception ex)
            {
                response.Mensagem = ex.Message;
                response.Status = false;
                return response;
            }
        }

        public async Task<ResponseModel<UsuarioModel>> RegistrarUsuario(UsuarioRegistrarDTO usuarioRegistrarDto)
        {
            var response = new ResponseModel<UsuarioModel>();
            try
            {
                if (!VerificarSeEmailValido(usuarioRegistrarDto.Email))
                {
                    response.Mensagem = "Digite um e-mail válido!";
                    response.Status = false;
                    return response;
                }

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

        public async Task<ResponseModel<UsuarioModel>> SolicitarRedefinicaoSenha(SolicitarRedefinicaoSenhaDTO solicitacaoDto)
        {
            var response = new ResponseModel<UsuarioModel>();

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == solicitacaoDto.Email);

            if (usuario == null)
            {
                response.Mensagem = "O e-mail digitado não está registrado. Tente novamente!";
                response.Status = false;
                return response;
            }

            var token = Guid.NewGuid().ToString().Substring(0, 8);
            usuario.Token = token;
            usuario.ExpiracaoToken = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            response.Mensagem = "Link de redefinição enviado para o seu e-mail.";
            response.Status = true;
            return response;
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

        private bool VerificarSeEmailValido(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}