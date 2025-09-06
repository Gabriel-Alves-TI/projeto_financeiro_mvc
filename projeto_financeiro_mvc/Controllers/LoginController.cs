using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Services.LoginService;
using projeto_financeiro_mvc.Services.SessaoService;

namespace projeto_financeiro_mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoginInterface _loginInterface;
        private readonly ISessaoInterface _sessaoInterface;

        public LoginController(AppDbContext context, ILoginInterface loginInterface, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _loginInterface = loginInterface;
            _sessaoInterface = sessaoInterface;
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
                return View("Index", usuarioLoginDto);
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}