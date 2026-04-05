using System.Reflection;
using Mecanica.Hermes.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mecanica.Hermes.Infrastructure.Persistence;

internal sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ClienteEntity> Clientes => Set<ClienteEntity>();
    public DbSet<VeiculoEntity> Veiculos => Set<VeiculoEntity>();
    public DbSet<ServicoEntity> Servicos => Set<ServicoEntity>();
    public DbSet<ProdutoEntity> Produtos => Set<ProdutoEntity>();
    public DbSet<OrdemDeServicoEntity> OrdensDeServico => Set<OrdemDeServicoEntity>();
    public DbSet<OrdemDeServicoStatusEntity> OrdemDeServicoStatus => Set<OrdemDeServicoStatusEntity>();
    public DbSet<OrdemDeServicoHistoricoStatusEntity> OrdemDeServicoHistoricoStatus => Set<OrdemDeServicoHistoricoStatusEntity>();
    public DbSet<OrdemDeServicoProdutoEntity> OrdemDeServicoProdutos => Set<OrdemDeServicoProdutoEntity>();
    public DbSet<OrdemDeServicoServicoEntity> OrdemDeServicoServicos => Set<OrdemDeServicoServicoEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pg_trgm");
        modelBuilder.HasPostgresExtension("unaccent");

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}