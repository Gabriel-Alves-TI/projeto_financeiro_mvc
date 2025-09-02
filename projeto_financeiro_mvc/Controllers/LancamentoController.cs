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
using projeto_financeiro_mvc.Views.Lancamento;

namespace projeto_financeiro_mvc.Controllers
{
    public class LancamentoController : Controller
    {
        private readonly AppDbContext _context;
        public LancamentoController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal)
        {
            var lancamentos = _context.Lancamentos
                .Include(l => l.Conta)
                .OrderBy(l => l.Data)
                .ToList();

            var transferencias = _context.Transferencias
                .Include(t => t.ContaOrigem)
                .Include(t => t.ContaDestino)
                .OrderBy(t => t.DataTransferencia)
                .ToList();

            var recorrentes = _context.Recorrentes
                .Include(r => r.Conta)
                .OrderBy(r => r.Data)
                .ToList();

            var contas = _context.Contas.ToList();

            if (!dataInicial.HasValue)
            {
                dataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            if (!dataFinal.HasValue)
            {
                dataFinal = dataInicial.Value.AddMonths(1).AddDays(-1);
            }

            lancamentos = lancamentos.Where(l => l.Data >= dataInicial.Value && l.Data <= dataFinal.Value).ToList();

            transferencias = transferencias.Where(t => t.DataTransferencia >= dataInicial.Value && t.DataTransferencia <= dataFinal.Value).ToList();

            recorrentes = recorrentes.Where(r => r.Data >= dataInicial.Value && r.Data <= dataFinal.Value).ToList();

            var viewModel = new MovimentosFinanceirosViewModel
            {
                Lancamentos = lancamentos,
                Transferencias = transferencias,
                Recorrentes = recorrentes,

                DataInicial = dataInicial,
                DataFinal = dataFinal,
                Contas = contas
            };

            return View(viewModel);
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
                Console.WriteLine($"Previsao: {viewModel.Lancamento.Previsao}");

                var conta = _context.Contas.Find(viewModel.Lancamento.ContaId);
                

                if (viewModel.Lancamento.Parcelas <= 0)
                {
                    TempData["MensagemErro"] = "Número de parcelas deve ser maior que 0.";
                    viewModel.Contas = _context.Contas.ToList();
                    return View(viewModel);
                }

                // Lógica para Ajuste de Saldo da Conta
                if (viewModel.Lancamento.Categoria?.Trim().Equals("Saldo Inicial", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (conta == null)
                    {
                        ModelState.AddModelError("", "Conta não localizada");
                        return View(viewModel);
                    }

                    conta.Saldo = 0;

                    if (viewModel.Lancamento.Tipo == TipoLancamento.Despesa)
                    {
                        conta.Saldo -= viewModel.Lancamento.Valor;
                    }
                    if (viewModel.Lancamento.Tipo == TipoLancamento.Receita)
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
                        ContaId = viewModel.Lancamento.ContaId
                    };

                    _context.Contas.Update(conta);

                    _context.Lancamentos.Add(lancamento);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Saldo inicial da conta corrigido com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                };                ;

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
                        ContaId = viewModel.Lancamento.ContaId
                    };

                    _context.Lancamentos.Add(lancamento);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Lançamento efetuado com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                }
                else
                {
                    var listaLancamentos = new List<LancamentoModel>();
                    DateTime dataParcela = viewModel.Lancamento.Data;

                    double valorParcela = Math.Round(viewModel.Lancamento.Valor / viewModel.Lancamento.Parcelas, 2);
                    
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

        public IActionResult Editar(int? id)
        {
            var lancamento = _context.Lancamentos.FirstOrDefault(lanc => lanc.Id == id);
            if (lancamento == null)
            {
                return NotFound();
            }

            var viewModel = new LancamentoViewModel
            {
                Lancamento = new LancamentoDTO
                {
                    Id = lancamento.Id,
                    Descricao = lancamento.Descricao,
                    Valor = lancamento.Valor,
                    Categoria = lancamento.Categoria,
                    Tipo = lancamento.Tipo,
                    Data = lancamento.Data,
                    Previsao = lancamento.Previsao,
                    Parcelas = lancamento.Parcelas,
                    Pago = lancamento.Pago,
                    ContaId = lancamento.ContaId
                },
                Contas = _context.Contas.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Editar(LancamentoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var conta = _context.Contas.Find(viewModel.Lancamento.ContaId);
                if (conta == null)
                {
                    TempData["MensagemErro"] = "Conta não localizada.";
                    return NotFound(viewModel);
                }

                var lancamento = _context.Lancamentos.Find(viewModel.Lancamento.Id);
                if (lancamento == null)
                {
                    TempData["MensagemErro"] = "Lançamento não encontrado.";
                    return NotFound(viewModel);
                }

                bool pagoAntes = lancamento.Pago;
                double valorAntes = lancamento.Valor;
                var tipoAntes = lancamento.Tipo;
                int? contaIdAntes = lancamento.ContaId;

                if (contaIdAntes != viewModel.Lancamento.ContaId)
                {
                    var contaAntiga = _context.Contas.Find(contaIdAntes);

                    if (pagoAntes)
                    {
                        if (tipoAntes == TipoLancamento.Receita)
                        {
                            contaAntiga.Saldo -= valorAntes;
                        }
                        if (tipoAntes == TipoLancamento.Despesa)
                        {
                            contaAntiga.Saldo += valorAntes;
                        }
                    }
                    _context.Contas.Update(contaAntiga);
                }

                if (pagoAntes != viewModel.Lancamento.Pago || valorAntes != viewModel.Lancamento.Valor || tipoAntes != viewModel.Lancamento.Tipo || contaIdAntes != viewModel.Lancamento.ContaId)
                {
                    if (pagoAntes)
                    {
                        if (tipoAntes == TipoLancamento.Receita)
                        {
                            conta.Saldo -= valorAntes;
                        }
                        if (tipoAntes == TipoLancamento.Despesa)
                        {
                            conta.Saldo += valorAntes;
                        }
                    }

                    if (viewModel.Lancamento.Pago)
                    {
                        if (viewModel.Lancamento.Tipo == TipoLancamento.Receita)
                        {
                            conta.Saldo += viewModel.Lancamento.Valor;
                        }
                        if (viewModel.Lancamento.Tipo == TipoLancamento.Despesa)
                        {
                            conta.Saldo -= viewModel.Lancamento.Valor;
                        }
                    }
                }

                lancamento.Descricao = viewModel.Lancamento.Descricao;
                lancamento.Valor = viewModel.Lancamento.Valor;
                lancamento.Categoria = viewModel.Lancamento.Categoria;
                lancamento.Tipo = viewModel.Lancamento.Tipo;
                lancamento.Data = viewModel.Lancamento.Data;
                lancamento.Previsao = viewModel.Lancamento.Previsao;
                lancamento.Parcelas = viewModel.Lancamento.Parcelas;
                lancamento.Pago = viewModel.Lancamento.Pago;
                lancamento.ContaId = viewModel.Lancamento.ContaId;

                _context.Contas.Update(conta);

                _context.Lancamentos.Update(lancamento);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Lançamento atualizado com sucesso!";
                return RedirectToAction("Index", "Lancamento");
            }
            viewModel.Contas = _context.Contas.ToList();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Pagar(LancamentoViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var conta = _context.Contas.Find(viewModel.Lancamento.ContaId);
                if (conta == null)
                {
                    TempData["MensagemErro"] = "Conta não localizada.";
                    return NotFound(viewModel);
                }

                var lancamento = _context.Lancamentos.Find(viewModel.Lancamento.Id);
                if (lancamento == null)
                {
                    TempData["MensagemErro"] = "Lançamento não localizado.";
                    return NotFound(viewModel);
                }

                lancamento.Pago = true;

                if (viewModel.Lancamento.Tipo == TipoLancamento.Receita)
                {
                    conta.Saldo += viewModel.Lancamento.Valor;
                }
                if (viewModel.Lancamento.Tipo == TipoLancamento.Despesa)
                {
                    conta.Saldo -= viewModel.Lancamento.Valor;
                }

                _context.Contas.Update(conta);
                _context.Lancamentos.Update(lancamento);
                _context.SaveChanges();
                return RedirectToAction("Index", "Lancamento");
            }

            return View(viewModel);
        }

        public IActionResult Excluir(int? id)
        {
            var lancamento = _context.Lancamentos
                .Include(l => l.Conta)
                .FirstOrDefault(l => l.Id == id);

            if (lancamento == null)
            {
                return NotFound();
            }

            return View(lancamento);
        }

        [HttpPost]
        public IActionResult Excluir(LancamentoModel lancamento)
        {
            var lancamentoDb = _context.Lancamentos
                .Include(l => l.Conta)
                .FirstOrDefault(l => l.Id == lancamento.Id);

            if (lancamentoDb == null)
            {
                return NotFound();
            }

            if (lancamentoDb.Pago == true)
            {
                if (lancamentoDb.Tipo == TipoLancamento.Receita)
                {
                    lancamentoDb.Conta.Saldo -= lancamentoDb.Valor;
                }
                if (lancamentoDb.Tipo == TipoLancamento.Despesa)
                {
                    lancamentoDb.Conta.Saldo += lancamentoDb.Valor;
                }

                _context.Contas.Update(lancamentoDb.Conta);
            };

            _context.Lancamentos.Remove(lancamentoDb);
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