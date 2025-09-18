using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class TransferenciaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;

        public TransferenciaController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Transferencia()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new TransferenciaViewModel
            {
                Transferencia = new TransferenciaDTO()
                {
                    DataTransferencia = DateTime.Today,
                    DataCompensacao = DateTime.Today
                },
                Contas = _context.Contas.Where(c => c.GrupoFamiliarId == usuario.GrupoFamiliarId && c.UsuarioId == usuario.Id).ToList(),
            };

            ViewBag.NomeUsuario = usuario.Nome;
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Transferencia(TransferenciaViewModel viewModel)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var contas = _context.Contas.Where(c => c.GrupoFamiliarId == usuario.GrupoFamiliarId && c.UsuarioId == usuario.Id).ToList();

            foreach (var erro in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine("===> ModelState com erros: " + erro.ErrorMessage);
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("===> ModelState com erros:");
                foreach (var erro in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(erro.ErrorMessage);
                }

                viewModel.Contas = contas;
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                var contaOrigem = _context.Contas.Find(viewModel.Transferencia.ContaOrigemId);
                var contaDestino = _context.Contas.Find(viewModel.Transferencia.ContaDestinoId);

                if (contaOrigem == null || contaDestino == null)
                {
                    ModelState.AddModelError("", "Conta origem ou destino não localizada.");
                    viewModel.Contas = _context.Contas.ToList();
                    return View(viewModel);
                }

                if (contaOrigem.Saldo < viewModel.Transferencia.Valor)
                {
                    ModelState.AddModelError("", "Saldo insuficiente na conta de origem.");

                    viewModel.Contas = contas;
                    return View(viewModel);
                }

                if (contaOrigem == contaDestino)
                {
                    ModelState.AddModelError("", "Selecione uma Conta Origem/Conta Destino diferente.");

                    viewModel.Contas = contas;
                    return View(viewModel);
                }

                var transferencia = new TransferenciaModel()
                {
                    Descricao = viewModel.Transferencia.Descricao,
                    Valor = viewModel.Transferencia.Valor,
                    Categoria = viewModel.Transferencia.Categoria,
                    Tipo = viewModel.Transferencia.Tipo,
                    DataTransferencia = viewModel.Transferencia.DataTransferencia,
                    DataCompensacao = viewModel.Transferencia.DataCompensacao,
                    ContaOrigemId = viewModel.Transferencia.ContaOrigemId,
                    ContaDestinoId = viewModel.Transferencia.ContaDestinoId,

                    UsuarioId = usuario.Id,
                    GrupoFamiliarId = usuario.GrupoFamiliarId,
                };

                contaOrigem.Saldo -= viewModel.Transferencia.Valor;
                contaDestino.Saldo += viewModel.Transferencia.Valor;

                _context.Contas.Update(contaOrigem);
                _context.Contas.Update(contaDestino);
                _context.Transferencias.Add(transferencia);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Transferência efetuada com sucesso!";
                return RedirectToAction("Index", "Lancamento");
            }

            viewModel.Contas = contas;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Visualizar(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            TransferenciaModel transferencia;

            if (usuario.GrupoFamiliarId.HasValue)
            {
                transferencia = _context.Transferencias
                    .Include(t => t.ContaOrigem)
                    .Include(t => t.ContaDestino)
                    .FirstOrDefault(t => t.UsuarioId == usuario.Id && t.GrupoFamiliarId == usuario.GrupoFamiliarId && t.Id == id);
            }
            else
            {
                transferencia = _context.Transferencias
                    .Include(t => t.ContaOrigem)
                    .Include(t => t.ContaDestino)
                    .FirstOrDefault(t => t.UsuarioId == usuario.Id && t.Id == id);
            }

            if (transferencia == null)
            {
                return NotFound();
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View(transferencia);
        }

        [HttpGet]
        public IActionResult Excluir(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }


            if (id == null || id == 0)
            {
                ModelState.AddModelError("", "Transferência não localizada");
                return NotFound();
            }

            TransferenciaModel transferencia = _context.Transferencias
                .Include(t => t.ContaOrigem)
                .Include(t => t.ContaDestino)
                .FirstOrDefault(transf => transf.Id == id);

            if (transferencia == null)
            {
                ModelState.AddModelError("", "Transferência não localizada");
                return NotFound();
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View(transferencia);
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            var transferencia = _context.Transferencias
                .Include(t => t.ContaOrigem)
                .Include(t => t.ContaDestino)
                .FirstOrDefault(t => t.Id == id);

            if (transferencia == null)
            {
                return NotFound();
            }

            transferencia.ContaOrigem.Saldo += transferencia.Valor;
            _context.Contas.Update(transferencia.ContaOrigem);

            transferencia.ContaDestino.Saldo -= transferencia.Valor;
            _context.Contas.Update(transferencia.ContaDestino);

            _context.Transferencias.Remove(transferencia);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Transferência excluída com sucesso!";
            return RedirectToAction("Index", "Lancamento");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}