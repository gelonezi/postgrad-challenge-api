using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Produtos.ValueObjects;

public class QuantidadeProdutoVoTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(1000)]
    public void Criar_ComQuantidadeValida_DeveRetornarSucesso(int quantidade)
    {
        var result = QuantidadeProdutoVo.Criar(quantidade);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be(quantidade);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Criar_ComQuantidadeInvalida_DeveRetornarErro(int quantidade)
    {
        var result = QuantidadeProdutoVo.Criar(quantidade);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("A quantidade do produto deve ser positiva.");
    }

}
