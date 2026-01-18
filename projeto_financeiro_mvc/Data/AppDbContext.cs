using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using projeto_financeiro_mvc.Models;
using projeto_financeiro_mvc.Models.Enums;

namespace projeto_financeiro_mvc.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ContaModel> Contas { get; set; }
        public DbSet<LancamentoModel> Lancamentos { get; set; }
        public DbSet<TransferenciaModel> Transferencias { get; set; }
        public DbSet<RecorrenteModel> Recorrentes { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }
        public DbSet<GrupoFamiliarModel> GrupoFamiliar { get; set; }
        public DbSet<CategoriaModel> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TransferenciaModel>()
                .HasOne(t => t.ContaOrigem)
                .WithMany()
                .HasForeignKey(t => t.ContaOrigemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferenciaModel>()
                .HasOne(t => t.ContaDestino)
                .WithMany()
                .HasForeignKey(t => t.ContaDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UsuarioModel>()
                .OwnsOne(u => u.Preferences, pref =>
                {
                    pref.Property(p => p.Theme)
                        .HasDefaultValue(Theme.Vapor);
                });
        }
    }
}