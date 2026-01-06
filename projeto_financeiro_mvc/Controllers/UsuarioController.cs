using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.Models.Enums;
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.Services.ThemeService;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;
        private readonly IThemeInterface _themeInterface;

        public UsuarioController(AppDbContext context, ISessaoInterface sessaoInterface, IThemeInterface themeInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
            _themeInterface = themeInterface;
        }

        public IActionResult Preferences()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View("Preferences/Index");
        }

        public IActionResult Theme()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var usuarioDb = _context.Usuarios
                .Include(u => u.Preferences)
                .FirstOrDefault(u => u.Id == usuario.Id);

            var viewModel = new UsuarioViewModel
            {
                UsuarioId = usuario.Id,
                Theme = usuarioDb.Preferences.Theme,
                ThemesDisponiveis = Enum.GetValues(typeof(Theme)).Cast<Theme>()
            };

            ViewBag.NomeUsuario = usuario.Nome;
            return View("Preferences/Theme", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeTheme(UsuarioViewModel viewModel)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!viewModel.Theme.HasValue)
            {
                ModelState.AddModelError("Theme", "Selecione um tema.");
                return View("Preferences/Theme", viewModel);
            }

            var changeTheme = await _themeInterface.ChangeTheme(viewModel.UsuarioId, viewModel.Theme.Value);

            TempData["MensagemSucesso"] = changeTheme.Mensagem;
            return RedirectToAction("Preferences");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}