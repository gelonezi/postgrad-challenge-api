using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.GetClienteById;

public sealed record GetClienteByIdQuery(Guid Id) : IRequest<Result<ClienteDto>>;