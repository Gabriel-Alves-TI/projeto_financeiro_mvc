using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class ExtratoController : Controller
    {
        private readonly AppDbContext _context;
        public ExtratoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal)
        {
            var listaContas = new List<ContaModel>();

            var contas = _context.Contas.ToList();

            var movimentos = new List<ExtratoViewModel>();

            movimentos.AddRange(_context.Lancamentos
                .Where(l => l.Pago == true)
                .Select(l => new ExtratoViewModel
                {
                    Id = l.Id,
                    Data = l.Data,
                    Descricao = l.Descricao,
                    Tipo = l.Tipo.ToString(),
                    Valor = l.Valor,
                    Categoria = l.Categoria,
                    Conta = l.Conta.Banco,
                    Origem = "Lancamento"
                }));

            movimentos.AddRange(_context.Recorrentes
                .Where(r => r.Pago == true)
                .Select(r => new ExtratoViewModel
                {
                    Id = r.Id,
                    Data = r.Data,
                    Descricao = r.Descricao,
                    Tipo = r.Tipo.ToString(),
                    Valor = r.Valor,
                    Categoria = r.Categoria,
                    Conta = r.Conta.Banco,
                    Origem = "Recorrente"
                }));

            movimentos.AddRange(_context.Transferencias.Select(t => new ExtratoViewModel
            {
                Id = t.Id,
                Data = t.DataTransferencia,
                Descricao = $"Transferência {t.ContaOrigem.Banco} -> {t.ContaDestino.Banco}",
                Tipo = t.Tipo.ToString(),
                Valor = t.Valor,
                Categoria = t.Categoria,
                Conta = t.ContaOrigem.Banco,
                Origem = "Transferencia"
            }));

            if (!dataInicial.HasValue)
            {
                dataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }

            if (!dataFinal.HasValue)
            {
                dataFinal = dataInicial.Value.AddMonths(1).AddDays(-1);
            }

            movimentos = movimentos
                .Where(m => m.Data >= dataInicial.Value && m.Data <= dataFinal.Value)
                .ToList();

            var viewModel = new ListExtratoViewModel
            {
                Movimentos = movimentos.OrderByDescending(m => m.Data).ToList(),

                DataInicial = dataInicial,
                DataFinal = dataFinal,
                Contas = contas
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}