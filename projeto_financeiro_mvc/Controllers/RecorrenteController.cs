using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.DTOs;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.ViewModels;
using projeto_financeiro_mvc.Views.Recorrente;

namespace projeto_financeiro_mvc.Controllers
{
    public class RecorrenteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;

        public RecorrenteController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
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

        [HttpGet]
        public IActionResult Editar(int? id)
        {
            var recorrente = _context.Recorrentes.FirstOrDefault(r => r.Id == id);
            if (recorrente == null)
            {
                TempData["MensagemErro"] = "Lançamento recorrente não localizado.";
                return NotFound();
            }

            var viewModel = new RecorrenteViewModel
            {
                Recorrente = new RecorrenteDTO
                {
                    Id = recorrente.Id,
                    Descricao = recorrente.Descricao,
                    Valor = recorrente.Valor,
                    Categoria = recorrente.Categoria,
                    Tipo = recorrente.Tipo,
                    Data = recorrente.Data,
                    Previsao = recorrente.Previsao,
                    Parcelas = recorrente.Parcelas,
                    Pago = recorrente.Pago,
                    IsRecorrente = recorrente.IsRecorrente,
                    ContaId = recorrente.ContaId
                },
                Contas = _context.Contas.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Editar(RecorrenteViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var conta = _context.Contas.Find(viewModel.Recorrente.ContaId);
                if (conta == null)
                {
                    TempData["MensagemErro"] = "Conta não localizada.";
                    return NotFound(viewModel);
                }

                var recorrente = _context.Recorrentes.Find(viewModel.Recorrente.Id);
                if (recorrente == null)
                {
                    TempData["MensagemErro"] = "Lançamento recorrente não localizado.";
                    return NotFound(viewModel);
                }

                if (viewModel.Recorrente.Pago == true)
                {
                    if (viewModel.Recorrente.Tipo == TipoLancamento.Receita)
                    {
                        conta.Saldo += viewModel.Recorrente.Valor;
                    }
                    if (viewModel.Recorrente.Tipo == TipoLancamento.Despesa)
                    {
                        conta.Saldo -= viewModel.Recorrente.Valor;
                    }
                }
                if (viewModel.Recorrente.Pago == false)
                {
                    if (viewModel.Recorrente.Tipo == TipoLancamento.Receita)
                    {
                        conta.Saldo -= viewModel.Recorrente.Valor;
                    }
                    if (viewModel.Recorrente.Tipo == TipoLancamento.Despesa)
                    {
                        conta.Saldo += viewModel.Recorrente.Valor;
                    }
                }

                recorrente.Descricao = viewModel.Recorrente.Descricao;
                recorrente.Valor = viewModel.Recorrente.Valor;
                recorrente.Categoria = viewModel.Recorrente.Categoria;
                recorrente.Tipo = viewModel.Recorrente.Tipo;
                recorrente.Data = viewModel.Recorrente.Data;
                recorrente.Previsao = viewModel.Recorrente.Previsao;
                recorrente.Parcelas = viewModel.Recorrente.Parcelas;
                recorrente.Pago = viewModel.Recorrente.Pago;
                recorrente.ContaId = viewModel.Recorrente.ContaId;

                _context.Contas.Update(conta);

                _context.Recorrentes.Update(recorrente);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Lançamento recorrente atualizado com sucesso!";
                return RedirectToAction("Index", "Lancamento");
            }

            viewModel.Contas = _context.Contas.ToList();
            return View(viewModel);
        }

        public IActionResult Excluir(int? id)
        {
            var recorrente = _context.Recorrentes
                .Include(c => c.Conta)
                .FirstOrDefault(r => r.Id == id);

            if (recorrente == null)
            {
                return NotFound();
            }

            return View(recorrente);
        }

        [HttpPost]
        public IActionResult Excluir(RecorrenteModel recorrente)
        {
            var recorrenteDb = _context.Recorrentes
                .Include(r => r.Conta)
                .FirstOrDefault(r => r.Id == recorrente.Id);

            if (recorrenteDb == null)
            {
                return NotFound();
            }

            if (recorrenteDb.Pago == true)
            {
                if (recorrenteDb.Tipo == TipoLancamento.Receita)
                {
                    recorrenteDb.Conta.Saldo -= recorrenteDb.Valor;
                }
                if (recorrenteDb.Tipo == TipoLancamento.Despesa)
                {
                    recorrenteDb.Conta.Saldo += recorrenteDb.Valor;
                }

                _context.Contas.Update(recorrenteDb.Conta);
            }

            _context.Recorrentes.Remove(recorrenteDb);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Lançamento recorrente excluído com sucesso!";
            return RedirectToAction("Index", "Lancamento");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}