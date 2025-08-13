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

namespace projeto_financeiro_mvc.Controllers
{
    public class ContaController : Controller
    {
        private readonly AppDbContext _context;

        public ContaController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult ListarContas()
        {
            ViewData["Title"] = "Contas";
            IEnumerable<ContaModel> contas = _context.Contas;

            return View(contas);
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(ContaDTO contaDTO)
        {
            if (ModelState.IsValid)
            {
                var conta = new ContaModel()
                {
                    Banco = contaDTO.Banco,
                    Agencia = contaDTO.Agencia,
                    NumeroConta = contaDTO.NumeroConta,
                    Saldo = contaDTO.SaldoInicial ?? 0,
                };

                _context.Contas.Add(conta);
                _context.SaveChanges();

                return RedirectToAction("ListarContas");
            }

            return View();
        }

        public IActionResult Editar(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ContaModel conta = _context.Contas.FirstOrDefault(c => c.Id == id);

            if (conta == null)
            {
                return NotFound();
            }

            return View(conta);
        }

        public IActionResult Excluir(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ContaModel conta = _context.Contas.FirstOrDefault(c => c.Id == id);

            if (conta == null)
            {
                return NotFound();
            }

            return View(conta);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }

}