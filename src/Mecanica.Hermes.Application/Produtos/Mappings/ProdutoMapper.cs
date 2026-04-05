using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Produtos.Mappings;

internal static class ProdutoMapper
{
    public static ProdutoDto ToDto(this Produto produto)
    {
        return new ProdutoDto(
            produto.Id,
            produto.Descricao.Valor,
            produto.Valor.Valor,
            produto.Quantidade.Valor,
            produto.Tipo.ToString());
    }
}
