using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
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
        private readonly IWebHostEnvironment _environment;

        public OfxController(AppDbContext context, ISessaoInterface sessaoInterface,IWebHostEnvironment environment)
        {
            _context = context;
            _sessaoInterface = sessaoInterface;
            _environment = environment;
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
            {
                TempData["MensagemErro"] = "Selecione um arquivo para fazer o upload.";
                return View("Index"); 
            }

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            var caminho = Path.Combine(pasta, arquivo.FileName);

            using (var stream = new FileStream(caminho, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            TempData["MensagemSucesso"] = "Arquivo enviado com sucesso.";
            return View("Index");
        }

        public IActionResult ReadOfx()//Testar passando o arquivo
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Testar path passando o arquivo: @"wwwroot/uploads/{arquivo}";
            string path = Path.Combine(
                _environment.WebRootPath,
                "uploads"
            );

            FileInfo ultimoArquivoOfx = new DirectoryInfo(path)
                                        .GetFiles()
                                        .OrderByDescending(f => f.CreationTime)
                                        .FirstOrDefault();
            
            Console.WriteLine(ultimoArquivoOfx);

            string caminhoOriginal = ultimoArquivoOfx.FullName;

            string conteudo = System.IO.File.ReadAllText(caminhoOriginal);

            conteudo = conteudo
                .Replace("&", "&amp;")
                .Replace("'", "&apos;");

            string caminhoCorrigido = Path.Combine(
                Path.GetTempPath(),
                $"ofx_corrigido_{Guid.NewGuid()}.ofx"
            );

            System.IO.File.WriteAllText(caminhoCorrigido, conteudo);

            if (ultimoArquivoOfx == null)
            {
                TempData["MensagemErro"] = "Nenhum arquivo localizado.";
                return RedirectToAction("Index");
            }
            
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
                // string pathOFX = ultimoArquivoOfx.ToString();
                Extract ofxParsed = OFXParser.Parser.GenerateExtract(caminhoCorrigido);

                var lancamentosOfx = new List<OfxViewModel>();

                if (ofxParsed != null)
                {
                    foreach (var transaction in ofxParsed.Transactions)
                    {
                        string[] splitDescription = transaction.Description.Split("-");


                        // Console.WriteLine(splitDescription[0]);
                        // Console.WriteLine(transaction.Description);
                        // Console.WriteLine(transaction.TransactionValue);
                        // Console.WriteLine(transaction.Date);

                        string Description = string.Empty;

                        //splitDescription cria array de string, Count > 3 para evitar IndexOutOfRangeException ao acessar index 1;
                        if (splitDescription.Count() >= 2)
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
        
        [RequestFormLimits(ValueCountLimit = 10000)]
        [HttpPost]
        public async Task<IActionResult> ImportarLancamentos(ListOfxViewModel model)
        {
            var usuario = _sessaoInterface.BuscarSessao();
            if (usuario == null)
            {
                return RedirectToAction("Index", "Login");
            }
            
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

                model.Contas = _context.Contas.ToList();
                model.Categorias = _context.Categorias.ToList();
                return View("_ImportarLancamentosOfx", model);
            }


            if (model.Contas == null || model.Categorias == null)
            {
                return RedirectToAction("Index", "Lancamento");
            }
            
            foreach (var lancamento in model.LancamentosOfx)
            {
                if (lancamento.CategoriaId == null)
                {
                    TempData["MensagemErro"] = $"Não é possível salvar sem uma Categoria selecionada ({lancamento.Descricao}).";
                    return View("_ImportarLancamentosOfx", model);
                }
            }

            if (model.LancamentosOfx == null || !model.LancamentosOfx.Any())
            {
                TempData["MensagemErro"] = "Não é possível importar uma lista de lançamentos vazia. Importe um novo arquivo e tente novamente.";
                return RedirectToAction("_ImportarLancamentosOfx");
            }

            var conta = model.ContaId;

            var contaDb = _context.Contas.FirstOrDefault(c => c.Id == model.ContaId);

            if (contaDb == null)
            {
                Console.WriteLine("Está caindo aqui!");
                TempData["MensagemErro"] = "Nenhuma conta localizada.";
                return RedirectToAction("_ImportarLancamentosOfx");
            }

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
                    Pago = true,
                    ContaId = conta,
                    UsuarioId = usuario.Id,
                    GrupoFamiliarId = usuario.GrupoFamiliarId
                };

                movimentos.Add(lancamento);
                Console.WriteLine(lancamento.CategoriaId);

                if (model.PagarTodos == true)
                {
                    if (lancamento.Tipo == TipoLancamento.Despesa)
                    {
                        contaDb.Saldo -= lancamento.Valor;
                    }
                    else if (lancamento.Tipo == TipoLancamento.Receita)
                    {
                        contaDb.Saldo += lancamento.Valor;
                    }

                    _context.Contas.Update(contaDb);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Lancamentos.AddRange(movimentos);
            await _context.SaveChangesAsync();

            Console.WriteLine("Está vindo para o final do metodo");
            return RedirectToAction("Index", "Lancamento");
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