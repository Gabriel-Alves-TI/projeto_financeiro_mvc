using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Services.EmailService;
using projeto_financeiro_mvc.Services.LoginService;
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoginInterface _loginInterface;
        private readonly ISessaoInterface _sessaoInterface;
        private readonly IEmailInterface _emailInterface;

        public LoginController(AppDbContext context, ILoginInterface loginInterface, ISessaoInterface sessaoInterface, IEmailInterface emailInterface)
        {
            _context = context;
            _loginInterface = loginInterface;
            _sessaoInterface = sessaoInterface;
            _emailInterface = emailInterface;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDTO usuarioLoginDto)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _loginInterface.Login(usuarioLoginDto);

                if (usuario.Status)
                {
                    Console.WriteLine(usuario.Mensagem);
                    return RedirectToAction("Index", "Dashboard");
                }
                else
                {
                    Console.WriteLine(usuario.Mensagem);
                    return View("Index", usuarioLoginDto);
                }
            }
            else
            {
                ModelState.AddModelError("", "Erro ao realizar login.");
                return View("Index");
            }
        }

        public IActionResult Logout()
        {
            _sessaoInterface.RemoverSessao();
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult Registrar()
        {
            return View(new UsuarioRegistrarDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Registrar(UsuarioRegistrarDTO usuarioRegistrarDto)
        {
            if (ModelState.IsValid)
            {
                Console.WriteLine("===> Dados recebidos:");
                Console.WriteLine($"Nome: {usuarioRegistrarDto.Nome}");
                Console.WriteLine($"Sobrenome: {usuarioRegistrarDto.Sobrenome}");
                Console.WriteLine($"Email: {usuarioRegistrarDto.Email}");
                Console.WriteLine($"SenhaHash: {usuarioRegistrarDto.Senha}");
                Console.WriteLine($"SenhaSalt: {usuarioRegistrarDto.ConfirmaSenha}");

                var usuario = await _loginInterface.RegistrarUsuario(usuarioRegistrarDto);

                if (usuario.Status)
                {
                    TempData["MensagemSucesso"] = usuario.Mensagem;
                }
                else
                {
                    TempData["MensagemErro"] = usuario.Mensagem;
                    return View(usuarioRegistrarDto);
                }

                return RedirectToAction("Index");
            }
            else
            {
                return View(usuarioRegistrarDto);
            }
        }

        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(RecuperacaoSenhaDTO solicitacaoDto)
        {
            var response = await _loginInterface.SolicitarRecuperacaoSenha(solicitacaoDto);
            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == solicitacaoDto.Email);

            if (!response.Status)
            {
                TempData["MensagemErro"] = response.Mensagem;
                return RedirectToAction("Index", "Login");
            }

            // Mensagem do E-mail
            string mensagem = @$"
                <p>Olá! Você recebeu um link para redefinição da sua senha.</p>
                <br>
                <p>Clique abaixo e crie a nova senha.</p>
                <br>
                <p><a href='http://localhost:5262/Login/RedefinirSenha?token={usuario.Token}' style='color:#1a73e8'>Redefinir senha </a></p>
                <br>
            ";

            await _emailInterface.EnviarEmailAsync(usuario.Email, "App Sistema Financeiro - Redefinição de senha", mensagem);    
            TempData["MensagemSucesso"] = response.Mensagem;
            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult RedefinirSenha(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["MensagemErro"] = "Token inválido.";
                return RedirectToAction("Index", "Login");
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == token);
            var expiracaoToken = usuario.ExpiracaoToken;
            DateTime dataAtual = DateTime.Now;

            Console.WriteLine(dataAtual.ToString("dd/MM/yyyy hh:mm"));
            Console.WriteLine(expiracaoToken.ToString());

            if (usuario == null || dataAtual > expiracaoToken)
            {
                TempData["MensagemErro"] = "Token inválido ou expirado.";
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new RedefinirSenhaViewModel
            {
                Email = usuario.Email,
                Token = token
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RedefinirSenha(RedefinicaoSenhaDTO redefinicaoDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _loginInterface.RedefinicaoSenha(redefinicaoDto);
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Token == redefinicaoDto.Token);

                if (!response.Status)
                {
                    TempData["MensagemErro"] = response.Mensagem;
                    return RedirectToAction("Index", "Login");
                }

                if (redefinicaoDto.NovaSenha != redefinicaoDto.ConfirmaNovaSenha)
                {
                    TempData["MensagemErro"] = "As senhas não conferem. Digite novamente.";
                    return View(redefinicaoDto);
                }
                else
                {
                    TempData["MensagemSucesso"] = response.Mensagem;
                    return RedirectToAction("Index", "Login");
                }
            }

            return View(redefinicaoDto);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}