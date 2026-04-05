using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveServico;

public sealed record RemoveServicoCommand(
    Guid OrdemDeServicoId,
    Guid ServicoId) : IRequest<Result>;
