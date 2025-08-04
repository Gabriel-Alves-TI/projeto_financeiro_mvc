using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class LancamentoController : Controller
    {
        private readonly AppDbContext _context;
        public LancamentoController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Cadastrar()
        {
            var viewModel = new LancamentoViewModel
            {
                Lancamento = new LancamentoDTO(),
                Contas = _context.Contas.ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Cadastrar(LancamentoViewModel viewModel)
        {
            Console.WriteLine("ContaId recebido: " + viewModel.Lancamento.ContaId);
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
                Console.WriteLine("===> Dados recebidos:");
                Console.WriteLine($"Categoria: {viewModel.Lancamento.Categoria}");
                Console.WriteLine($"ContaId: {viewModel.Lancamento.ContaId}");
                Console.WriteLine($"Descricao: {viewModel.Lancamento.Descricao}");
                Console.WriteLine($"Valor: {viewModel.Lancamento.Valor}");
                Console.WriteLine($"Tipo: {viewModel.Lancamento.Tipo}");
                Console.WriteLine($"Data: {viewModel.Lancamento.Data}");
                Console.WriteLine($"Parcelas: {viewModel.Lancamento.Parcelas}");
                Console.WriteLine($"Recorrente: {viewModel.Lancamento.Recorrente}");
                Console.WriteLine($"Previsao: {viewModel.Lancamento.Previsao}");

                var conta = _context.Contas.Find(viewModel.Lancamento.ContaId);
                if (conta == null)
                {
                    TempData["MensagemErro"] = "Conta não localizada.";
                    return View(viewModel);
                }

                if (viewModel.Lancamento.Parcelas <= 0)
                {
                    TempData["MensagemErro"] = "Número de parcelas deve ser maior que 0.";
                    return View(viewModel);
                }

                if (viewModel.Lancamento.Categoria?.Trim().Equals("Saldo Inicial", StringComparison.OrdinalIgnoreCase) == true)
                {
                    Console.Write("Entrou aqui");

                    if (viewModel.Lancamento.Tipo.ToLower() == "despesa")
                    {
                        conta.Saldo -= viewModel.Lancamento.Valor;
                    }
                    if (viewModel.Lancamento.Tipo.ToLower() == "receita")
                    {
                        conta.Saldo += viewModel.Lancamento.Valor;
                    }

                    var lancamento = new LancamentoModel()
                    {
                        Descricao = viewModel.Lancamento.Descricao,
                        Valor = viewModel.Lancamento.Valor,
                        Categoria = viewModel.Lancamento.Categoria,
                        Tipo = viewModel.Lancamento.Tipo,
                        Data = viewModel.Lancamento.Data,
                        Previsao = viewModel.Lancamento.Previsao,
                        Parcelas = viewModel.Lancamento.Parcelas,
                        Pago = true,
                        Recorrente = viewModel.Lancamento.Recorrente,
                        ContaId = viewModel.Lancamento.ContaId
                    };

                    _context.Contas.Update(conta);

                    _context.Lancamentos.Add(lancamento);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Saldo inicial da conta corrigido com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                };

                if (viewModel.Lancamento.Parcelas == 1)
                {
                    var lancamento = new LancamentoModel()
                    {
                        Descricao = viewModel.Lancamento.Descricao,
                        Valor = viewModel.Lancamento.Valor,
                        Categoria = viewModel.Lancamento.Categoria,
                        Tipo = viewModel.Lancamento.Tipo,
                        Data = viewModel.Lancamento.Data,
                        Previsao = viewModel.Lancamento.Previsao,
                        Parcelas = viewModel.Lancamento.Parcelas,
                        Pago = false,
                        Recorrente = viewModel.Lancamento.Recorrente,
                        ContaId = viewModel.Lancamento.ContaId
                    };

                    _context.Lancamentos.Add(lancamento);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Lançamento efetuado com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                }
            }

            viewModel.Contas = _context.Contas.ToList();
            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}