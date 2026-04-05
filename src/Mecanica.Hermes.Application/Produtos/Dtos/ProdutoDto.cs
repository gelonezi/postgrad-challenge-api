namespace Mecanica.Hermes.Application.Produtos.Dtos;

public sealed record ProdutoDto(
    Guid Id,
    string Descricao,
    decimal Valor,
    int Quantidade,
    string Tipo);
