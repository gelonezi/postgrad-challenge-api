using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.DeleteVeiculo;

public sealed record DeleteVeiculoCommand(Guid ClienteId, Guid VeiculoId) : IRequest<Result>;
