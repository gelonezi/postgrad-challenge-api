using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Commands.AddVeiculo;

public sealed record AddVeiculoCommand(
    Guid ClienteId,
    string Modelo,
    string Marca,
    string Placa,
    int Ano) : IRequest<Result<ClienteDto>>;