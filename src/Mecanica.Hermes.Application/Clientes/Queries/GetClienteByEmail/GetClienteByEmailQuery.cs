using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.GetClienteByEmail;

public sealed record GetClienteByEmailQuery(string Email) : IRequest<Result<ClienteDto>>;