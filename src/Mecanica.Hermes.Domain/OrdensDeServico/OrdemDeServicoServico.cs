using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Domain.OrdensDeServico;

public class OrdemDeServicoServico : BaseDomain
{
    internal OrdemDeServicoServico()
    {
    }

    public Guid ServicoId { get; private set; }
    public string Descricao { get; private set; } = null!;
    public decimal Valor { get; private set; }

    internal static OrdemDeServicoServico Criar(Servico servico)
    {
        return new OrdemDeServicoServico
        {
            ServicoId = servico.Id,
            Descricao = servico.Descricao.Valor,
            Valor = servico.Valor.Valor
        };
    }

    internal static OrdemDeServicoServico Restaurar(
        Guid id,
        Guid servicoId,
        string descricao,
        decimal valor,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        var item = new OrdemDeServicoServico
        {
            ServicoId = servicoId,
            Descricao = descricao,
            Valor = valor
        };
        item.RestaurarBase(id, createdAt, updatedAt, isDeleted);
        return item;
    }
}