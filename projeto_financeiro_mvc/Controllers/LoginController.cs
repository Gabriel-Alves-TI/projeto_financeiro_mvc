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

namespace projeto_financeiro_mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILoginInterface _loginInterface;

        public LoginController(AppDbContext context, ILoginInterface loginInterface)
        {
            _context = context;
            _loginInterface = loginInterface;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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