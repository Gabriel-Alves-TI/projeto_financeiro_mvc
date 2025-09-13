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
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.ViewModels;
using projeto_financeiro_mvc.Views.Lancamento;

namespace projeto_financeiro_mvc.Controllers
{
    public class LancamentoController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;
        public LancamentoController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
        }
        [HttpGet]
        public IActionResult Index(DateTime? dataInicial, DateTime? dataFinal, string? tipo, string? descricao, string? categoria, int? contaId, double? valor)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
            var contas = _context.Contas
                .Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .ToList();

            var movimentos = new List<Lancamentos>();

            movimentos.AddRange(_context.Lancamentos
                .Where(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .Select(l => new Lancamentos
                {
                    Id = l.Id,
                    Data = l.Data,
                    Descricao = l.Descricao,
                    Tipo = l.Tipo,
                    Valor = l.Valor,
                    Categoria = l.Categoria,
                    Pago = l.Pago,
                    Conta = l.Conta.Banco,
                    ContaId = l.ContaId,
                    ContaDestino = null,
                    IsRecorrente = null,
                    Origem = "Lancamento"
                }));

            movimentos.AddRange(_context.Recorrentes
                .Where(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .Select(r => new Lancamentos
                {
                    Id = r.Id,
                    Data = r.Data,
                    Descricao = r.Descricao,
                    Tipo = r.Tipo,
                    Valor = r.Valor,
                    Categoria = r.Categoria,
                    Pago = r.Pago,
                    Conta = r.Conta.Banco,
                    ContaId = r.ContaId,
                    ContaDestino = null,
                    IsRecorrente = true,
                    Origem = "Recorrente"
                }));

            movimentos.AddRange(_context.Transferencias
                .Where(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .Select(t => new Lancamentos
                {
                    Id = t.Id,
                    Data = t.DataTransferencia,
                    Descricao = $"Transferência {t.ContaOrigem.Banco} -> {t.ContaDestino.Banco}",
                    Tipo = t.Tipo,
                    Valor = t.Valor,
                    Categoria = t.Categoria,
                    Pago = true,
                    Conta = t.ContaOrigem.Banco,
                    ContaId = t.ContaOrigemId,
                    ContaDestino = t.ContaDestino.Banco,
                    IsRecorrente = null,
                    Origem = "Transferencia"
                }));

            if (!dataInicial.HasValue)
            {
                dataInicial = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            }
            if (!dataFinal.HasValue)
            {
                dataFinal = dataInicial.Value.AddMonths(1).AddDays(-1);
            }
            if (!string.IsNullOrEmpty(tipo))
            {
                movimentos = movimentos.Where(m => m.Tipo.ToString() == tipo).ToList();
            }
            if (!string.IsNullOrEmpty(descricao))
            {
                movimentos = movimentos.Where(m => m.Descricao.Equals(descricao, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(categoria))
            {
                movimentos = movimentos.Where(m => m.Categoria.Equals(categoria, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (contaId.HasValue)
            {
                movimentos = movimentos.Where(m => m.ContaId == contaId.Value).ToList();
            }
            if (valor.HasValue)
            {
                movimentos = movimentos.Where(m => m.Valor == valor.Value).ToList();
            }

            movimentos = movimentos.Where(l => l.Data >= dataInicial.Value && l.Data <= dataFinal.Value).ToList();

            var viewModel = new ListLancamentosViewModel
            {
                Movimentos = movimentos.OrderByDescending(m => m.Data).ToList(),

                DataInicial = dataInicial,
                DataFinal = dataFinal,
                Contas = contas
            };

            return View(viewModel);
        }

        public IActionResult Cadastrar()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var viewModel = new LancamentoViewModel
            {
                Lancamento = new LancamentoDTO()
                {
                    Data = DateTime.Today,
                    Previsao = DateTime.Today
                },
                Contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Cadastrar(LancamentoViewModel viewModel)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

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
                        ContaId = viewModel.Lancamento.ContaId,

                        UsuarioId = usuario.Id,
                        GrupoFamiliarId = usuario.GrupoFamiliarId,
                    };

                    _context.Contas.Update(conta);

                    _context.Lancamentos.Add(lancamento);
                    _context.SaveChanges();

                    TempData["MensagemSucesso"] = "Saldo inicial da conta corrigido com sucesso!";
                    return RedirectToAction("Index", "Lancamento");
                };                

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
                        ContaId = viewModel.Lancamento.ContaId,

                        UsuarioId = usuario.Id,
                        GrupoFamiliarId = usuario.GrupoFamiliarId,
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
                            ContaId = viewModel.Lancamento.ContaId,

                            UsuarioId = usuario.Id,
                            GrupoFamiliarId = usuario.GrupoFamiliarId,
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
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var lancamento = _context.Lancamentos
                .Where(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .FirstOrDefault(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId && l.Id == id);
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
                Contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Editar(LancamentoViewModel viewModel)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (ModelState.IsValid)
            {
                var lancamento = _context.Lancamentos
                    .FirstOrDefault(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId && l.Id == viewModel.Lancamento.Id);
                if (lancamento == null)
                {
                    // TempData["MensagemErro"] = "Lançamento não encontrado.";

                    ModelState.AddModelError("", "Lançamento recorrente não localizado.");

                    viewModel.Contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId).ToList();
                    return NotFound(viewModel);
                }

                if (viewModel.Lancamento.Pago == true && viewModel.Lancamento.ContaId == null)
                {
                    ModelState.AddModelError("", "Não é possível realizar um pagamento sem uma conta selecionada!");
                    viewModel.Contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId).ToList();
                    return View(viewModel);
                }

                if (viewModel.Lancamento.Pago == true && viewModel.Lancamento.ContaId == null)
                {
                    ModelState.AddModelError("", "Não é possível realizar um pagamento sem uma conta selecionada!");
                    viewModel.Contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id).ToList();
                    return View(viewModel);
                }

                bool pagoAntes = lancamento.Pago;
                double valorAntes = lancamento.Valor;
                var tipoAntes = lancamento.Tipo;
                int? contaIdAntes = lancamento.ContaId;
                int? contaIdNova = null;

                if (pagoAntes && contaIdAntes.HasValue)
                {
                    var contaAntiga = _context.Contas.Find(contaIdAntes.Value);
                    if (tipoAntes == TipoLancamento.Receita)
                        contaAntiga.Saldo -= valorAntes;
                    else
                        contaAntiga.Saldo += valorAntes;

                    _context.Contas.Update(contaAntiga);
                }

                if (viewModel.Lancamento.Pago && viewModel.Lancamento.ContaId.HasValue)
                {
                    var contaNova = _context.Contas.Find(viewModel.Lancamento.ContaId.Value);
                    if (viewModel.Lancamento.Tipo == TipoLancamento.Receita)
                        contaNova.Saldo += viewModel.Lancamento.Valor;
                    else
                        contaNova.Saldo -= viewModel.Lancamento.Valor;

                    _context.Contas.Update(contaNova);
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

                _context.Lancamentos.Update(lancamento);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Lançamento atualizado com sucesso!";
                return RedirectToAction("Index", "Lancamento");
            }
            viewModel.Contas = _context.Contas.Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId).ToList();
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
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            LancamentoModel lancamento;
            if (usuario.GrupoFamiliarId.HasValue)
            {
                lancamento = _context.Lancamentos
                    .Include(l => l.Conta)
                    .FirstOrDefault(l => l.Id == id && l.GrupoFamiliarId == usuario.GrupoFamiliarId);
            }
            else
            {
                lancamento = _context.Lancamentos
                    .Include(l => l.Conta)
                    .FirstOrDefault(l => l.Id == id && l.UsuarioId == usuario.Id);
            }

            if (lancamento == null)
            {
                return NotFound();
            }

            return View(lancamento);
        }

        [HttpPost]
        public IActionResult Excluir(LancamentoModel lancamento)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var lancamentoDb = _context.Lancamentos
                .Include(l => l.Conta)
                .FirstOrDefault(l => l.UsuarioId == usuario.Id && l.GrupoFamiliarId == usuario.GrupoFamiliarId && l.Id == lancamento.Id);

            if (lancamentoDb == null)
            {
                return NotFound();
            }

            if (lancamentoDb.Categoria.Equals("saldo inicial", StringComparison.OrdinalIgnoreCase) == true)
            {
                if (lancamentoDb.Pago == true && lancamentoDb.Conta != null)
                {
                    lancamentoDb.Conta.Saldo -= lancamentoDb.Valor;

                    _context.Contas.Update(lancamentoDb.Conta);
                }

                _context.Lancamentos.Remove(lancamentoDb);
                _context.SaveChanges();

                TempData["MensagemSucesso"] = "Saldo inicial removido com sucesso!";
                return RedirectToAction("Index");
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