using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var lancamentos = _context.Lancamentos
                .Include(lanc => lanc.Conta)
                .OrderBy(lanc => lanc.Data)
                .Select(lanc => new LancamentoModel
                {
                    Id = lanc.Id,
                    Descricao = lanc.Descricao,
                    Valor = lanc.Valor,
                    Categoria = lanc.Categoria,
                    Tipo = lanc.Tipo,
                    Data = lanc.Data,
                    Previsao = lanc.Previsao,
                    Parcelas = lanc.Parcelas,
                    Pago = lanc.Pago,
                    Recorrente = lanc.Recorrente,
                    ContaId = lanc.ContaId,
                    Conta = lanc.Conta
                }).ToList();

            return View(lancamentos);
        }

        public IActionResult Cadastrar()
        {
            var viewModel = new LancamentoViewModel
            {
                Lancamento = new LancamentoDTO()
                {
                    Data = DateTime.Today,
                    Previsao = DateTime.Today
                },
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

                // Lógica para Ajuste de Saldo da Conta
                if (viewModel.Lancamento.Categoria?.Trim().Equals("Saldo Inicial", StringComparison.OrdinalIgnoreCase) == true)
                {

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
                }
                ;

                // Lógica para Lançamento único
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

                
                var listaLancamentos = new List<LancamentoModel>();
                DateTime dataParcela = viewModel.Lancamento.Data;

                double valorParcela = Math.Round(viewModel.Lancamento.Valor / viewModel.Lancamento.Parcelas, 2);

                // Lógica para Recorrente
                if (viewModel.Lancamento.Recorrente == true)
                {
                    for (int i = 1; i <= viewModel.Lancamento.Parcelas; i++)
                    {
                        var lancamento = new LancamentoModel()
                        {
                            Descricao = $"{viewModel.Lancamento.Descricao} - Recorrente({i}/{viewModel.Lancamento.Parcelas})",
                            Valor = viewModel.Lancamento.Valor,
                            Categoria = viewModel.Lancamento.Categoria,
                            Tipo = viewModel.Lancamento.Tipo,
                            Data = dataParcela,
                            Previsao = viewModel.Lancamento.Previsao,
                            Parcelas = viewModel.Lancamento.Parcelas,
                            Pago = false,
                            Recorrente = viewModel.Lancamento.Recorrente,
                            ContaId = viewModel.Lancamento.ContaId
                        };

                        listaLancamentos.Add(lancamento);

                        dataParcela = dataParcela.AddMonths(1);
                    }

                    _context.AddRange(listaLancamentos);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Lançamento recorrente efetuado com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                }
                else
                {
                    // Lógica para Parcelamento
                    for (int i = 1; i <= viewModel.Lancamento.Parcelas; i++)
                    {
                        var lancamento = new LancamentoModel()
                        {
                            Descricao = $"{viewModel.Lancamento.Descricao} - Parcelamento({i}/{viewModel.Lancamento.Parcelas})",
                            Valor = valorParcela,
                            Categoria = viewModel.Lancamento.Categoria,
                            Tipo = viewModel.Lancamento.Tipo,
                            Data = dataParcela,
                            Previsao = viewModel.Lancamento.Previsao,
                            Parcelas = viewModel.Lancamento.Parcelas,
                            Pago = false,
                            Recorrente = viewModel.Lancamento.Recorrente,
                            ContaId = viewModel.Lancamento.ContaId
                        };

                        listaLancamentos.Add(lancamento);

                        dataParcela = dataParcela.AddMonths(1);
                    }

                    _context.AddRange(listaLancamentos);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Parcelamento efetuado com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                }
            }

            viewModel.Contas = _context.Contas.ToList();
            return View(viewModel);
        }

        public IActionResult Excluir(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            LancamentoModel lancamento = _context.Lancamentos.FirstOrDefault(lanc => lanc.Id == id);

            if (lancamento == null)
            {
                return NotFound();
            }

            return View(lancamento);
        }

        [HttpPost]
        public IActionResult Excluir(LancamentoModel lancamento)
        {
            Console.Write("Lancamento Id recebido: ", lancamento.Id);
            if (lancamento == null)
            {
                return NotFound();
            }

            _context.Lancamentos.Remove(lancamento);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Lançamento excluído com sucesso!";
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}