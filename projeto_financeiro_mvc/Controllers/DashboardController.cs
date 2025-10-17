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
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;

        public DashboardController(AppDbContext context, ISessaoInterface sessaoInterface)
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

            var lancamentos = _context.Lancamentos
                .Include(lanc => lanc.Conta)
                .Include(lanc => lanc.Categoria)
                .Where(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId && l.Categoria.Descricao != "Saldo Inicial")
                .OrderBy(lanc => lanc.Data)
                .ToList();
            
            var recorrentes = _context.Recorrentes
                .Where(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .Include(r => r.Conta)
                .OrderBy(r => r.Data)
                .ToList();

            var contas = _context.Contas
                .Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .ToList();

            var (inicioSemana, fimSemana) = GetIntervaloSemana(DateTime.Today);

            var receitasLancamentos = lancamentos
                .Where(l => l.Tipo == TipoLancamento.Receita && l.Pago == true && l.Data >= inicioSemana && l.Data <= fimSemana)
                .Select(l => new {l.Data, l.Valor})
                .ToList();

            var receitasRecorrentes = recorrentes
                .Where(r => r.Tipo == TipoLancamento.Receita && r.Pago == true && r.Data >= inicioSemana && r.Data <= fimSemana)
                .Select(r => new {r.Data, r.Valor})
                .ToList();

            var receitas = receitasLancamentos.Concat(receitasRecorrentes);

            var despesasLancamentos = lancamentos
                .Where(l => l.Tipo == TipoLancamento.Despesa && l.Pago == true && l.Data >= inicioSemana && l.Data <= fimSemana)
                .Select(l => new {l.Data, l.Valor})
                .ToList();

            var despesasRecorrentes = recorrentes
                .Where(r => r.Tipo == TipoLancamento.Despesa && r.Pago == true && r.Data >= inicioSemana && r.Data <= fimSemana)
                .Select(r => new {r.Data, r.Valor})
                .ToList();

            var despesasSemanais = despesasLancamentos.Concat(despesasRecorrentes);

            var diasSemana = new[] {"Dom", "Seg", "Ter", "Qua", "Quin", "Sex", "Sab"};

            var receitaSemanal = diasSemana.Select((_, i) =>
                receitas.Where(l => (int)l.Data.DayOfWeek == i)
                    .Sum(l => l.Valor)
            ).ToList();

            var despesaSemanal = diasSemana.Select((_, i) =>
                despesasSemanais.Where(l => (int)l.Data.DayOfWeek == i)
                    .Sum(l => l.Valor)
            ).ToList();


            var viewModel = new MovimentosFinanceirosViewModel
            {
                Lancamentos = lancamentos,
                Recorrentes = recorrentes,
                Contas = contas,
                DiasSemana = diasSemana,
                ReceitaSemanal = receitaSemanal,
                DespesaSemanal = despesaSemanal
            };

            ViewBag.NomeUsuario = usuario.Nome;
            return View(viewModel);
        }

        public (DateTime inicio, DateTime fim) GetIntervaloSemana(DateTime dataBase, bool semanaComecaSegunda = true)
        {
            // Descobre o "índice" do dia da semana
            int diaSemana = (int)dataBase.DayOfWeek;

            // Se a semana começa na segunda, ajusta
            if (semanaComecaSegunda)
            {
                diaSemana = diaSemana == 0 ? 6 : diaSemana - 1;
            }

            // Primeiro dia da semana
            DateTime inicio = dataBase.Date.AddDays(-diaSemana);

            // Último dia da semana 
            DateTime fim = inicio.AddDays(6);

            return (inicio, fim);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}