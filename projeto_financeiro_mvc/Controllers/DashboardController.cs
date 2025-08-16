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
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var viewModel = new MovimentosFinanceirosViewModel
            {
                Lancamentos = _context.Lancamentos
                    .Include(lanc => lanc.Conta)
                    .OrderBy(lanc => lanc.Data)
                    .ToList(),
                
                Recorrentes = _context.Recorrentes
                    .Include(r => r.Conta)
                    .OrderBy(r => r.Data)
                    .ToList(),
                
                Contas = _context.Contas
                    .ToList()
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