using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.UpdateVeiculo;

public sealed record UpdateVeiculoCommand(
    Guid ClienteId,
    Guid VeiculoId,
    string Modelo,
    string Marca,
    int Ano) : IRequest<Result<ClienteDto>>;