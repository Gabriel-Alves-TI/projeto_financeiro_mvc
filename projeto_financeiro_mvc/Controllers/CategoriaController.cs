using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Services.SessaoService;

namespace projeto_financeiro_mvc.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;

        public CategoriaController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Index()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var categorias = _context.Categorias
                .Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .ToList();

            ViewBag.NomeUsuario = usuario.Nome;
            return View(categorias);
        }

        public IActionResult Cadastrar()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(CategoriaDTO categoriaDto)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                var categoria = new CategoriaModel
                {
                    Descricao = categoriaDto.Descricao,

                    UsuarioId = usuario.Id,
                    GrupoFamiliarId = usuario.GrupoFamiliarId
                };

                _context.Categorias.Add(categoria);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Categoria cadastrada com sucesso";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["MensagemErro"] = "Ocorreu algum erro ao salvar nova categoria.";
                return View(categoriaDto);
            }
        }

        public IActionResult Editar(int id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                TempData["MensagemErro"] = "Categoria não localizada.";
                return RedirectToAction("Index");
            }

            var categoriaDto = new CategoriaDTO
            {
                Id = categoria.Id,
                Descricao = categoria.Descricao
            };

            ViewBag.NomeUsuario = usuario.Nome;
            return View(categoriaDto);
        }

        [HttpPost]
        public IActionResult Editar(CategoriaDTO categoriaDto)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == categoriaDto.Id);
            if (categoria == null)
            {
                TempData["MensagemErro"] = "Categoria não localizada.";
                return RedirectToAction("Index");
            }

            categoria.Descricao = categoriaDto.Descricao;

            _context.Categorias.Update(categoria);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Categoria atualizada com sucesso.";
            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria == null)
            {
                TempData["MensagemErro"] = "Categoria não localizada.";
                return RedirectToAction("Index");
            }

            var categoriaDto = new CategoriaDTO
            {
                Id = categoria.Id,
                Descricao = categoria.Descricao
            };

            ViewBag.NomeUsuario = usuario.Nome;
            return View(categoriaDto);
        }

        [HttpPost]
        public IActionResult Excluir(CategoriaDTO categoriaDto)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == categoriaDto.Id);
            if (categoria == null)
            {
                TempData["MensagemErro"] = "Categoria não localizada.";
                return RedirectToAction("Index");
            }

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Categoria excluída com sucesso.";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}