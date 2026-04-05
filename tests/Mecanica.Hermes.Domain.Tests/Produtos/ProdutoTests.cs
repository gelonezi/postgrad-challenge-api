using Mecanica.Hermes.Domain.Produtos;
using Mecanica.Hermes.Domain.Produtos.Enums;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Produtos;

public class ProdutoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarProdutoComEvento()
    {
        var result = Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca");

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Valor.Should().Be("Óleo de Motor");
        result.Data.Valor.Valor.Should().Be(50.00m);
        result.Data.Quantidade.Valor.Should().Be(10);
        result.Data.Tipo.Should().Be(TipoProduto.Peca);
        result.Data.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void AtualizarDados_DeveModificarProdutoEAdicionarEvento()
    {
        var produto = CriarProdutoPadrao();

        var result = produto.AtualizarDados("Óleo Sintético", 75.00m);

        result.IsSuccess.Should().BeTrue();
        produto.Descricao.Valor.Should().Be("Óleo Sintético");
        produto.Valor.Valor.Should().Be(75.00m);
        produto.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void AdicionarEstoque_DeveAumentarQuantidadeEAdicionarEvento()
    {
        var produto = CriarProdutoPadrao();
        var adicional = QuantidadeProdutoVo.Criar(5).Data!;
        var quantidadeInicial = produto.Quantidade.Valor;

        var result = produto.AdicionarEstoque(adicional);

        result.IsSuccess.Should().BeTrue();
        produto.Quantidade.Valor.Should().Be(quantidadeInicial + 5);
        produto.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void RemoverEstoque_ComQuantidadeSuficiente_DeveReduzirQuantidadeEAdicionarEvento()
    {
        var produto = CriarProdutoPadrao();
        var remover = QuantidadeProdutoVo.Criar(3).Data!;
        var quantidadeInicial = produto.Quantidade.Valor;

        var result = produto.RemoverEstoque(remover);

        result.IsSuccess.Should().BeTrue();
        produto.Quantidade.Valor.Should().Be(quantidadeInicial - 3);
        produto.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void RemoverEstoque_ComQuantidadeInsuficiente_DeveRetornarErroEAdicionarEvento()
    {
        var produto = CriarProdutoPadrao();
        var remover = QuantidadeProdutoVo.Criar(20).Data!;
        var quantidadeInicial = produto.Quantidade.Valor;

        var result = produto.RemoverEstoque(remover);

        result.IsFailure.Should().BeTrue();
        produto.Quantidade.Valor.Should().Be(quantidadeInicial);
        produto.DomainEvents.Should().HaveCount(2);
    }

    private static Produto CriarProdutoPadrao()
    {
        return Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca").Data!;
    }
}
