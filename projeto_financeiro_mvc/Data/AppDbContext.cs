using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using projeto_financeiro_mvc.Models;

namespace projeto_financeiro_mvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ContaModel> Contas { get; set; }
        public DbSet<LancamentoModel> Lancamentos { get; set; }
    }
}