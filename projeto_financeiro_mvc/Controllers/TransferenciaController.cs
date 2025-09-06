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
            var viewModel = new TransferenciaViewModel
            {
                Transferencia = new TransferenciaDTO()
                {
                    DataTransferencia = DateTime.Today,
                    DataCompensacao = DateTime.Today
                },
                Contas = _context.Contas.ToList(),
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Transferencia(TransferenciaViewModel viewModel)
        {
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

                viewModel.Contas = _context.Contas.ToList();
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
                    Console.WriteLine("Saldo Insuficiente");
                    ModelState.AddModelError("", "Saldo insuficiente na conta de origem.");
                    viewModel.Contas = _context.Contas.ToList();
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
                    ContaDestinoId = viewModel.Transferencia.ContaDestinoId
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

            viewModel.Contas = _context.Contas.ToList();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencia = _context.Transferencias.FirstOrDefault(t => t.Id == id);

            if (transferencia == null)
            {
                return NotFound();
            }

            var viewModel = new TransferenciaViewModel
            {
                Transferencia = new TransferenciaDTO
                {
                    Id = transferencia.Id,
                    Descricao = transferencia.Descricao,
                    Valor = transferencia.Valor,
                    Categoria = transferencia.Categoria,
                    Tipo = transferencia.Tipo,
                    DataTransferencia = transferencia.DataTransferencia,
                    DataCompensacao = transferencia.DataCompensacao,
                    ContaOrigemId = transferencia.ContaOrigemId,
                    ContaDestinoId = transferencia.ContaDestinoId
                },
                Contas = _context.Contas.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]//PRECISA CORRIGIR QUANDO ALTERAR CONTAS
        public IActionResult Editar(TransferenciaViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["MensagemErro"] = "Dados inválidos.";
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                var transferenciaBanco = _context.Transferencias
                    .Include(t => t.ContaOrigem)
                    .Include(t => t.ContaDestino)
                    .FirstOrDefault(t => t.Id == viewModel.Transferencia.Id);

                if (transferenciaBanco == null)
                {
                    ModelState.AddModelError("", "Transferência não localizada");
                    return NotFound();
                }

                transferenciaBanco.ContaOrigem.Saldo += transferenciaBanco.Valor;
                transferenciaBanco.ContaDestino.Saldo -= transferenciaBanco.Valor;

                var novaContaOrigem = _context.Contas.First(c => c.Id == viewModel.Transferencia.ContaOrigemId);
                var novaContaDestino = _context.Contas.First(c => c.Id == viewModel.Transferencia.ContaDestinoId);

                novaContaOrigem.Saldo -= viewModel.Transferencia.Valor;
                novaContaDestino.Saldo += viewModel.Transferencia.Valor;

                transferenciaBanco.Descricao = viewModel.Transferencia.Descricao;
                transferenciaBanco.Valor = viewModel.Transferencia.Valor;
                transferenciaBanco.DataTransferencia = viewModel.Transferencia.DataTransferencia;
                transferenciaBanco.DataCompensacao = viewModel.Transferencia.DataCompensacao;
                transferenciaBanco.ContaOrigem = novaContaOrigem;
                transferenciaBanco.ContaDestino = novaContaDestino;

                _context.Transferencias.Update(transferenciaBanco);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Transferência atualizada com sucesso!";
                return RedirectToAction("Index", "Lancamento");
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Excluir(int? id)
        {
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