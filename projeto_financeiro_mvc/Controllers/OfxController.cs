using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OFXParser.Entities;
using projeto_financeiro_mvc.Data;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Services.SessaoService;
using projeto_financeiro_mvc.ViewModels;

namespace projeto_financeiro_mvc.Controllers
{
    public class OfxController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ISessaoInterface _sessaoInterface;

        public OfxController(AppDbContext context, ISessaoInterface sessaoInterface)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
        }

        public IActionResult Index()
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            ViewBag.NomeUsuario = usuario.Nome;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var caminho = Path.Combine(pasta, arquivo.FileName);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            TempData["MensagemSucesso"] = "Arquivo enviado com sucesso.";
            return View("_ModalImportarOfx");
        }

        public IActionResult ReadOfx()//Testar passando o arquivo
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Testar path passando o arquivo: @"wwwroot/uploads/{arquivo}";
            var path = @"wwwroot/uploads/nubank-ofx.ofx";
            FileStream fs = null;
            StreamReader sr = null;

            var contas = _context.Contas
                .Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .ToList();

            var categorias = _context.Categorias
                .Where(c => c.UsuarioId == usuario.Id && c.GrupoFamiliarId == usuario.GrupoFamiliarId)
                .ToList();

            try
            {
                string pathOFX = path;
                Extract ofxParsed = OFXParser.Parser.GenerateExtract(pathOFX);

                var lancamentosOfx = new List<OfxViewModel>();

                if (ofxParsed != null)
                {
                    foreach (var transaction in ofxParsed.Transactions)
                    {
                        string[] splitDescription = transaction.Description.Split("-");


                        Console.WriteLine(splitDescription[0]);
                        Console.WriteLine(transaction.Description);
                        // Console.WriteLine(transaction.TransactionValue);
                        // Console.WriteLine(transaction.Date);

                        string Description = string.Empty;

                        //splitDescription cria array de string, Count > 3 para evitar IndexOutOfRangeException ao acessar index 1;
                        if (splitDescription.Count() > 3)
                        {
                            Description += splitDescription[0] + "-" + splitDescription[1];
                            Console.WriteLine(Description);
                        }
                        else
                        {
                            Description += splitDescription[0];
                            Console.WriteLine(Description);
                        }

                        var tipo = transaction.TransactionValue >= 0 ?
                            TipoLancamento.Receita :
                            TipoLancamento.Despesa;
                        
                        var lancamentoOfx = new OfxViewModel
                        {
                            Descricao = Description,
                            Valor = transaction.TransactionValue,
                            CategoriaId = null,
                            Tipo = tipo,
                            Data = transaction.Date,
                            Previsao = transaction.Date,
                            Parcelas = 1,
                            Pago = false,
                            ContaId = null,
                        };

                        lancamentosOfx.Add(lancamentoOfx);
                    }

                    var viewModel = new ListOfxViewModel
                    {
                        LancamentosOfx = lancamentosOfx.OrderByDescending(l => l.Data).ToList(),
                        Contas = contas,
                        Categorias = categorias
                    };

                    ExcluirArquivos();

                    ViewBag.NomeUsuario = usuario.Nome;
                    return View("_ImportarLancamentosOfx", viewModel);
                }

                ExcluirArquivos();
                return View("Index");               
            }
            catch (IOException e)
            {
                Console.WriteLine("An error occurred");
                Console.WriteLine(e.Message);
                TempData["Error"] = $"Erro ao ler arquivo OFX: {e.Message}";
                return RedirectToAction("Index");
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportarLancamentos(ListOfxViewModel model)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (model.LancamentosOfx == null || !model.LancamentosOfx.Any())
            {
                TempData["MensagemErro"] = "Não é possível importar uma lista de lançamentos vazia. Importe um novo arquivo e tente novamente.";

                return RedirectToAction("_ImportarLancamentosOfx");
            }

            var conta = model.ContaId;

            var movimentos = new List<LancamentoModel>();

            foreach (var item in model.LancamentosOfx)
            {
                var tipo = item.Valor >= 0 ?
                    TipoLancamento.Receita :
                    TipoLancamento.Despesa;

                var lancamento = new LancamentoModel
                {
                    Descricao = item.Descricao,
                    Valor = Math.Abs(item.Valor),
                    CategoriaId = item.CategoriaId,
                    Tipo = tipo,
                    Data = item.Data,
                    Previsao = item.Previsao,
                    Parcelas = item.Parcelas,
                    Pago = item.Pago,
                    ContaId = conta,
                    UsuarioId = usuario.Id,
                    GrupoFamiliarId = usuario.GrupoFamiliarId
                };

                movimentos.Add(lancamento);
                Console.WriteLine(lancamento.CategoriaId);
            }

            _context.Lancamentos.AddRange(movimentos);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private void ExcluirArquivos()
        {
            string path = @"wwwroot\uploads\";

            string[] filesXml = Directory.GetFiles(path, "*.xml");

            foreach (var file in filesXml)
            {
                if (System.IO.File.Exists(file))
                {
                    System.IO.File.Delete(file);
                }
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}