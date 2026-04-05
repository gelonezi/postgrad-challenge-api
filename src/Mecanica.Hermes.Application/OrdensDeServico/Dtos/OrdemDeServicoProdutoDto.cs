namespace Mecanica.Hermes.Application.OrdensDeServico.Dtos;

public sealed record OrdemDeServicoProdutoDto(
    Guid Id,
    Guid ProdutoId,
    string Descricao,
    decimal Valor,
    int Quantidade,
    string Tipo);
