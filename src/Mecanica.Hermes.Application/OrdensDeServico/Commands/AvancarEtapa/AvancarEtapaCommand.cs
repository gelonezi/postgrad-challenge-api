using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AvancarEtapa;

public sealed record AvancarEtapaCommand(Guid OrdemDeServicoId) : IRequest<Result<OrdemDeServicoDto>>;
