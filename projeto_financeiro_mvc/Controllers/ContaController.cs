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
using projeto_financeiro_mvc.Services.SessaoService;

namespace projeto_financeiro_mvc.Controllers
{
    public class ContaController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;

        public ContaController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult ListarContas()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            IQueryable<ContaModel> contas;

            if (usuario.GrupoFamiliarId.HasValue)
            {
                // Retorna todas as contas do grupo
                contas = _context.Contas
                    .Where(c => c.GrupoFamiliarId == usuario.GrupoFamiliarId);
            }
            else
            {
                // Usuário individual, retorna apenas as dele
                contas = _context.Contas
                    .Where(c => c.UsuarioId == usuario.Id);
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View(contas.ToList());
        }

        public IActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Cadastrar(ContaDTO contaDto)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                var conta = new ContaModel()
                {
                    Banco = contaDto.Banco,
                    Agencia = contaDto.Agencia,
                    NumeroConta = contaDto.NumeroConta,
                    Saldo = contaDto.SaldoInicial ?? 0,

                    UsuarioId = usuario.Id,
                    GrupoFamiliarId = usuario.GrupoFamiliarId
                };

                _context.Contas.Add(conta);
                _context.SaveChanges();

                return RedirectToAction("ListarContas");
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View(contaDto);
        }

        public IActionResult Editar(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (id == null)
            {
                return NotFound();
            }

            ContaModel conta;

            if (usuario.GrupoFamiliarId.HasValue)
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == id && c.GrupoFamiliarId == usuario.GrupoFamiliarId);
            }
            else
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuario.Id);
            }

            if (conta == null)
            {
                return NotFound();
            }

            var contaDto = new ContaDTO
            {
                Banco = conta.Banco,
                Agencia = conta.Agencia,
                NumeroConta = conta.NumeroConta,
                SaldoInicial = conta.Saldo
            };

            ViewBag.NomeUsuario = usuario.Nome;
            return View(contaDto);
        }

        [HttpPost]
        public IActionResult Editar(ContaDTO contaDto)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View(contaDto);
            }

            ContaModel conta;

            if (usuario.GrupoFamiliarId.HasValue)
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == contaDto.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId);
            }
            else
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == contaDto.Id && c.UsuarioId == usuario.Id);
            }

            if (conta == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(contaDto.Banco) || string.IsNullOrEmpty(contaDto.Agencia) || string.IsNullOrEmpty(contaDto.NumeroConta))
            {
                return View(contaDto);
            }

            conta.Banco = contaDto.Banco;
            conta.Agencia = contaDto.Agencia;
            conta.NumeroConta = contaDto.NumeroConta;

            _context.Contas.Update(conta);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Conta atualizada com sucesso!";

            var contaDtoAtualizado = new ContaDTO
            {
                Id = conta.Id,
                Banco = conta.Banco,
                Agencia = conta.Agencia,
                NumeroConta = conta.NumeroConta,
                SaldoInicial = conta.Saldo
            };

            return RedirectToAction("ListarContas");
        }

        public IActionResult Excluir(int? id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ContaModel conta;
            if (usuario.GrupoFamiliarId.HasValue)
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == id && c.GrupoFamiliarId == usuario.GrupoFamiliarId);
            }
            else
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuario.Id);
            }

            if (conta == null)
            {
                return NotFound();
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View(conta);
        }

        [HttpPost]
        public IActionResult Excluir(int id)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ContaModel conta;
            if (usuario.GrupoFamiliarId.HasValue)
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == id && c.GrupoFamiliarId == usuario.GrupoFamiliarId);
            }
            else
            {
                conta = _context.Contas
                    .FirstOrDefault(c => c.Id == id && c.UsuarioId == usuario.Id);
            }

            if (conta == null)
            {
                TempData["MensagemErro"] = "Conta não encontrada ou você não tem permissão para excluir.";
                return RedirectToAction("ListarContas");
            }

            var lancamentos = _context.Lancamentos.Where(l => l.ContaId == conta.Id);
            if (lancamentos.Any())
            {
                TempData["MensagemErro"] = "Não foi possível excluir porque a conta possui lançamentos vinculados.";
                return RedirectToAction("ListarContas");
            }

            _context.Contas.Remove(conta);
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