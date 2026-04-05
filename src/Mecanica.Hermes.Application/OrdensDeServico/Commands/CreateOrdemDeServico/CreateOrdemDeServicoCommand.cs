using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.CreateOrdemDeServico;

public sealed record CreateOrdemDeServicoCommand(
    Guid ClienteId,
    Guid VeiculoId,
    string? ProblemaRelatado,
    string? Observacoes) : IRequest<Result<OrdemDeServicoDto>>;
