using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.CancelarOrdemDeServico;

public sealed record CancelarOrdemDeServicoCommand(Guid OrdemDeServicoId) : IRequest<Result<OrdemDeServicoDto>>;
