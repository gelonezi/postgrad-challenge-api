using Mecanica.Hermes.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mecanica.Hermes.Infrastructure.Persistence.Configurations;

internal class OrdemDeServicoConfiguration : IEntityTypeConfiguration<OrdemDeServicoEntity>
{
    public void Configure(EntityTypeBuilder<OrdemDeServicoEntity> builder)
    {
        builder.ToTable("OrdensDeServico");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ClienteId)
            .IsRequired();

        builder.Property(x => x.VeiculoId)
            .IsRequired();

        builder.Property(x => x.ProblemaRelatado)
            .HasColumnName("ProblemaRelatado")
            .HasMaxLength(1000);

        builder.Property(x => x.Observacoes)
            .HasColumnName("Observacoes")
            .HasMaxLength(1000);

        builder.Property(x => x.StatusAtualId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.HasOne(x => x.Cliente)
            .WithMany()
            .HasForeignKey(x => x.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veiculo)
            .WithMany()
            .HasForeignKey(x => x.VeiculoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.StatusAtual)
            .WithMany()
            .HasForeignKey(x => x.StatusAtualId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.HistoricosStatus)
            .WithOne(x => x.OrdemDeServico)
            .HasForeignKey(x => x.OrdemDeServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Produtos)
            .WithOne(x => x.OrdemDeServico)
            .HasForeignKey(x => x.OrdemDeServicoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Servicos)
            .WithOne(x => x.OrdemDeServico)
            .HasForeignKey(x => x.OrdemDeServicoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
