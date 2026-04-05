using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.UpdateCliente;

public sealed record UpdateClienteCommand(
    Guid Id,
    string NomeCivil,
    string? NomeSocial,
    string Email,
    string Telefone) : IRequest<Result<ClienteDto>>;