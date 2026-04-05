using System.ComponentModel.DataAnnotations;

namespace Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;

public sealed record AddVeiculoRequest(
    [Required(AllowEmptyStrings = false)] string Modelo,
    [Required(AllowEmptyStrings = false)] string Marca,
    [Required(AllowEmptyStrings = false)] string Placa,
    [Required(AllowEmptyStrings = false)] int Ano);