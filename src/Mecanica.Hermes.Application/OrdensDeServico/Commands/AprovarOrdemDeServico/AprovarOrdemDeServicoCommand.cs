using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AprovarOrdemDeServico;

public sealed record AprovarOrdemDeServicoCommand(Guid OrdemDeServicoId) : IRequest<Result<OrdemDeServicoDto>>;