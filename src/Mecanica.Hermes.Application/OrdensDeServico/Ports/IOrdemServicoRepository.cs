using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;

namespace Mecanica.Hermes.Application.OrdensDeServico.Ports;

public interface IOrdemServicoRepository
{
    Task<OrdemDeServico?> GetByIdAsync(Guid id);
    Task<PagedResult<OrdemDeServico>> ListByStatusAsync(OrdemDeServicoStatus status, PaginationParams paginationParams);
    Task<PagedResult<OrdemDeServico>> ListAllAsync(PaginationParams paginationParams);
    Task AddAsync(OrdemDeServico servico);
    void Update(OrdemDeServico servico);
}