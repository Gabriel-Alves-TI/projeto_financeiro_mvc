using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
            var conta = _context.Contas.FirstOrDefault(c => c.Id == id);
            if (conta == null)
            {
                return NotFound();
            }

            ContaDTO contaDto = new ContaDTO
            {
                Banco = conta.Banco,
                Agencia = conta.Agencia,
                NumeroConta = conta.NumeroConta,
                SaldoInicial = conta.Saldo
            };

            return View(contaDto);
        }

        [HttpPost]
        public IActionResult Editar(ContaDTO contaDto)
        {
            if (!ModelState.IsValid)
            {
                return View(contaDto);
            }

            var conta = _context.Contas.Find(contaDto.Id);
            if (conta == null)
            {
                return NotFound();
            }

            if (conta.Banco.IsNullOrEmpty() || conta.Agencia.IsNullOrEmpty() || conta.NumeroConta.IsNullOrEmpty())
            {
                return View(contaDto);
            }

            conta.Banco = contaDto.Banco;
            conta.Agencia = contaDto.Agencia;
            conta.NumeroConta = contaDto.NumeroConta;

            _context.Contas.Update(conta);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Conta atualizada com sucesso!";
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

        [HttpPost]
        public IActionResult Excluir(ContaModel conta)
        {
            var contaId = _context.Contas.FirstOrDefault(c => c.Id == conta.Id);
            if (contaId == null)
            {
                return NotFound();
            }

            var lancamentos = _context.Lancamentos.Where(l => l.ContaId == conta.Id);
            if (lancamentos.Any())
            {
                TempData["MensagemErro"] = "Não foi possível excluir porque a conta possui lançamentos vinculados.";
                return RedirectToAction("ListarContas");
            }

            _context.Contas.Remove(contaId);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Conta excluída com sucesso!";
            return RedirectToAction("ListarContas");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }

}