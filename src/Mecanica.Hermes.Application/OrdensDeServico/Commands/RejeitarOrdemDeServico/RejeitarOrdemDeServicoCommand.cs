using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.RejeitarOrdemDeServico;

public sealed record RejeitarOrdemDeServicoCommand(Guid OrdemDeServicoId) : IRequest<Result<OrdemDeServicoDto>>;