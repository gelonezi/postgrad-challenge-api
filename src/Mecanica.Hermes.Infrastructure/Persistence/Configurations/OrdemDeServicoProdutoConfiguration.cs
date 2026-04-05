using Mecanica.Hermes.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Mecanica.Hermes.Infrastructure.Persistence.Configurations;

internal class OrdemDeServicoProdutoConfiguration : IEntityTypeConfiguration<OrdemDeServicoProdutoEntity>
{
    public void Configure(EntityTypeBuilder<OrdemDeServicoProdutoEntity> builder)
    {
        builder.ToTable("OrdemDeServicoProdutos");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrdemDeServicoId)
            .IsRequired();

        builder.Property(x => x.ProdutoId)
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("Descricao")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Valor)
            .HasColumnName("Valor")
            .IsRequired();

        builder.Property(x => x.Quantidade)
            .HasColumnName("Quantidade")
            .IsRequired();

        builder.Property(x => x.Tipo)
            .HasConversion<string>()
            .HasColumnName("Tipo")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.IsDeleted);
    }
}
