
using Junto.Dominio;

using Microsoft.EntityFrameworkCore;

namespace Junto.Infra.Data
{
    public class BancoDbContext : DbContext
    {
        public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options)
        { }

        public DbSet<Seguros> Seguros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Seguros>()
                .HasKey(p => p.Id);
        }
    }
}