using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Produtos.ValueObjects;

public class ValorProdutoVoTests
{
    [Theory]
    [InlineData(10.50)]
    [InlineData(0)]
    [InlineData(1000.99)]
    public void Criar_ComValorValido_DeveRetornarSucesso(decimal valor)
    {
        var result = ValorProdutoVo.Criar(valor);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be(valor);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100.50)]
    public void Criar_ComValorNegativo_DeveRetornarErro(decimal valor)
    {
        var result = ValorProdutoVo.Criar(valor);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Valor do produto não pode ser negativo.");
    }

    [Theory]
    [InlineData(10.999, 10.99)]
    [InlineData(25.555, 25.55)]
    [InlineData(100.001, 100.00)]
    public void Criar_ComMaisDeDuasCasasDecimais_DeveLimitarADuasCasas(decimal valor, decimal esperado)
    {
        var result = ValorProdutoVo.Criar(valor);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Valor.Should().Be(esperado);
    }
}
