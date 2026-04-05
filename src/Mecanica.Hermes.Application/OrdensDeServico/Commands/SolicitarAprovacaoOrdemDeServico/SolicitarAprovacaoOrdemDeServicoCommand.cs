using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.SolicitarAprovacaoOrdemDeServico;

public sealed record SolicitarAprovacaoOrdemDeServicoCommand(Guid OrdemDeServicoId) : IRequest<Result>;