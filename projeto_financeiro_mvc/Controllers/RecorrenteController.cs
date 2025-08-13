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
using projeto_financeiro_mvc.ViewModels;
using projeto_financeiro_mvc.Views.Recorrente;

namespace projeto_financeiro_mvc.Controllers
{
    public class RecorrenteController : Controller
    {
        private readonly AppDbContext _context;

        public RecorrenteController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Cadastrar()
        {
            var viewModel = new RecorrenteViewModel
            {
                Recorrente = new RecorrenteDTO()
                {
                    Data = DateTime.Today,
                    Previsao = DateTime.Today,
                },
                Contas = _context.Contas.ToList()
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Cadastrar(RecorrenteViewModel viewModel)
        {
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
                Console.WriteLine($"Categoria: {viewModel.Recorrente.Categoria}");
                Console.WriteLine($"ContaId: {viewModel.Recorrente.ContaId}");
                Console.WriteLine($"Descricao: {viewModel.Recorrente.Descricao}");
                Console.WriteLine($"Valor: {viewModel.Recorrente.Valor}");
                Console.WriteLine($"Tipo: {viewModel.Recorrente.Tipo}");
                Console.WriteLine($"Data: {viewModel.Recorrente.Data}");
                Console.WriteLine($"Parcelas: {viewModel.Recorrente.Parcelas}");
                Console.WriteLine($"Previsao: {viewModel.Recorrente.Previsao}");

                var conta = _context.Contas.Find(viewModel.Recorrente.ContaId);
                if (conta == null)
                {
                    TempData["MensagemErro"] = "Conta não localizada.";
                    return View(viewModel);
                }

                if (viewModel.Recorrente.IsRecorrente == false && viewModel.Recorrente.Parcelas == 1)
                {
                    var recorrente = new RecorrenteModel()
                    {
                        Descricao = viewModel.Recorrente.Descricao,
                        Valor = viewModel.Recorrente.Valor,
                        Categoria = viewModel.Recorrente.Categoria,
                        Tipo = viewModel.Recorrente.Tipo,
                        Data = viewModel.Recorrente.Data,
                        Previsao = viewModel.Recorrente.Previsao,
                        Parcelas = viewModel.Recorrente.Parcelas,
                        Pago = false,
                        IsRecorrente = viewModel.Recorrente.IsRecorrente,
                        ContaId = viewModel.Recorrente.ContaId
                    };

                    _context.Recorrentes.Add(recorrente);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Lançamento único efetuado com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                }

                var listaLancamentos = new List<RecorrenteModel>();
                DateTime dataParcela = viewModel.Recorrente.Data;

                if (viewModel.Recorrente.IsRecorrente == true &&  viewModel.Recorrente.Parcelas > 1)
                {
                    for (int i = 1; i <= viewModel.Recorrente.Parcelas; i++)
                    {
                        var recorrente = new RecorrenteModel()
                        {
                            Descricao = $"{viewModel.Recorrente.Descricao} - Recorrente({i}/{viewModel.Recorrente.Parcelas})",
                            Valor = viewModel.Recorrente.Valor,
                            Categoria = viewModel.Recorrente.Categoria,
                            Tipo = viewModel.Recorrente.Tipo,
                            Data = dataParcela,
                            Previsao = viewModel.Recorrente.Previsao,
                            Parcelas = viewModel.Recorrente.Parcelas,
                            Pago = false,
                            IsRecorrente = viewModel.Recorrente.IsRecorrente,
                            ContaId = viewModel.Recorrente.ContaId
                        };

                        listaLancamentos.Add(recorrente);

                        dataParcela = dataParcela.AddMonths(1);
                    }

                    _context.AddRange(listaLancamentos);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Lançamento recorrente efetuado com sucesso!";
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