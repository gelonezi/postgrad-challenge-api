using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Application.Produtos.Ports;

public interface IProdutoRepository
{
    Task<Produto?> GetByIdAsync(Guid id);
    Task<PagedResult<Produto>> ListByDescricaoAsync(DescricaoProdutoVo descricao, PaginationParams paginationParams);
    Task<PagedResult<Produto>> ListAllAsync(PaginationParams paginationParams);
    Task AddAsync(Produto servico);
    void Update(Produto servico);
}