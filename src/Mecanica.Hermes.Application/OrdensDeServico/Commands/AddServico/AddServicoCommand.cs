using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AddServico;

public sealed record AddServicoCommand(
    Guid OrdemDeServicoId,
    Guid ServicoId) : IRequest<Result<OrdemDeServicoDto>>;
