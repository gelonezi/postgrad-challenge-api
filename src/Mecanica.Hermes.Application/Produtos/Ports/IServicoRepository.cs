using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Application.Produtos.Ports;

public interface IServicoRepository
{
    Task<Servico?> GetByIdAsync(Guid id);
    Task<PagedResult<Servico>> ListByDescricaoAsync(DescricaoProdutoVo descricao, PaginationParams paginationParams);
    Task<PagedResult<Servico>> ListAllAsync(PaginationParams paginationParams);
    Task AddAsync(Servico servico);
    void Update(Servico servico);
}