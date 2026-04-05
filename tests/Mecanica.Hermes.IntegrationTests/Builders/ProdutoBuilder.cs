using Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;
using Mecanica.Hermes.Domain.Produtos.Enums;

namespace Mecanica.Hermes.IntegrationTests.Builders;

public class ProdutoBuilder
{
    private string _descricao = "Produto Teste";
    private decimal _valor = 50.00m;
    private int _quantidade = 10;
    private TipoProduto _tipo = TipoProduto.Peca;

    public ProdutoBuilder ComDescricao(string descricao)
    {
        _descricao = descricao;
        return this;
    }

    public ProdutoBuilder ComValor(decimal valor)
    {
        _valor = valor;
        return this;
    }

    public ProdutoBuilder ComQuantidade(int quantidade)
    {
        _quantidade = quantidade;
        return this;
    }

    public ProdutoBuilder DoTipo(TipoProduto tipo)
    {
        _tipo = tipo;
        return this;
    }

    public AddProdutoRequest Build()
    {
        return new AddProdutoRequest(_descricao, _valor, _quantidade, _tipo);
    }
}
