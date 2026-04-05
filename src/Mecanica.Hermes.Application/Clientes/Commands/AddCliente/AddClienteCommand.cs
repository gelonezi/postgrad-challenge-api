using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.AddCliente;

public sealed record AddClienteCommand(
    string NomeCivil,
    string? NomeSocial,
    string IdentificacaoFiscal,
    string Email,
    string Telefone) : IRequest<Result<ClienteDto>>;