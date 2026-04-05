using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.DeleteCliente;

public sealed record DeleteClienteCommand(Guid Id) : IRequest<Result>;
