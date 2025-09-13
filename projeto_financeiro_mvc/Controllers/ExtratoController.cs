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
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class ExtratoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;
        public ExtratoController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
        }

        [HttpGet]
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal, string? tipo, string? categoria, string? descricao, double? valor, int? contaId)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var listaContas = new List<ContaModel>();

            var contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId).ToList();

            var movimentos = new List<ExtratoViewModel>();

            if (contaId.HasValue)
            {
                movimentos.AddRange(_context.Lancamentos
                    .Where(l => l.UsuarioId == usuario.Id
                    && l.GrupoFamiliarId == usuario.GrupoFamiliarId
                    && l.Pago == true
                    && l.ContaId == contaId.Value)
                    .Select(l => new ExtratoViewModel
                    {
                        Id = l.Id,
                        Data = l.Data,
                        Descricao = l.Descricao,
                        Tipo = l.Tipo.ToString(),
                        Valor = l.Valor,
                        Categoria = l.Categoria,
                        Conta = l.Conta.Banco,
                        ContaId = l.ContaId,
                        Origem = "Lancamento"
                    }));

                movimentos.AddRange(_context.Recorrentes
                    .Where(r => r.UsuarioId == usuario.Id
                    && r.GrupoFamiliarId == usuario.GrupoFamiliarId
                    && r.Pago == true
                    && r.ContaId == contaId.Value)
                    .Select(r => new ExtratoViewModel
                    {
                        Id = r.Id,
                        Data = r.Data,
                        Descricao = r.Descricao,
                        Tipo = r.Tipo.ToString(),
                        Valor = r.Valor,
                        Categoria = r.Categoria,
                        Conta = r.Conta.Banco,
                        ContaId = r.ContaId,
                        Origem = "Recorrente"
                    }));

                movimentos.AddRange(_context.Transferencias
                    .Where(t => t.UsuarioId == usuario.Id
                    && t.GrupoFamiliarId == usuario.GrupoFamiliarId
                    && t.ContaOrigemId == contaId.Value || t.ContaDestinoId == contaId.Value)
                    .Select(t => new ExtratoViewModel
                    {
                        Id = t.Id,
                        Data = t.DataTransferencia,
                        Descricao = $"Transferência {t.ContaOrigem.Banco} -> {t.ContaDestino.Banco}",
                        Tipo = t.Tipo.ToString(),
                        Valor = t.Valor,
                        Categoria = t.Categoria,
                        Conta = t.ContaOrigem.Banco,
                        ContaId = t.ContaOrigemId,
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

                if (!string.IsNullOrEmpty(tipo))
                {
                    movimentos = movimentos.Where(m => m.Tipo == tipo).ToList();
                }

                if (!string.IsNullOrEmpty(categoria))
                {
                    movimentos = movimentos.Where(m => m.Categoria == categoria).ToList();
                }

                if (!string.IsNullOrEmpty(descricao))
                {
                    movimentos = movimentos.Where(m => m.Descricao.Equals(descricao, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (valor.HasValue)
                {
                    Console.WriteLine("valor recebido: " + valor.Value);
                    movimentos = movimentos.Where(m => m.Valor == valor.Value).ToList();
                }

                movimentos = movimentos
                    .Where(m => m.Data >= dataInicial.Value && m.Data <= dataFinal.Value)
                    .ToList();
            }
 
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