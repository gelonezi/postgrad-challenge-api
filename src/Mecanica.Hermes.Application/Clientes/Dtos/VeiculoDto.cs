namespace Mecanica.Hermes.Application.Clientes.Dtos;

public sealed record VeiculoDto(
    Guid Id,
    string Modelo,
    string Marca,
    string Placa,
    int Ano);