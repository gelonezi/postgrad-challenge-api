namespace Mecanica.Hermes.Application.Produtos.Dtos;

public sealed record ServicoDto(
    Guid Id,
    string Descricao,
    decimal Valor);
