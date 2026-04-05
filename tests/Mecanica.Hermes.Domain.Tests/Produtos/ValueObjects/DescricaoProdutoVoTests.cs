using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Produtos.ValueObjects;

public class DescricaoProdutoVoTests
{
    [Theory]
    [InlineData("óleo de motor", "Óleo de Motor")]
    [InlineData("FILTRO DE AR", "Filtro de Ar")]
    [InlineData("  pneu  radial  ", "Pneu Radial")]
    public void Criar_ComDescricaoValida_DeveRetornarSucessoENormalizado(string descricao, string esperado)
    {
        var result = DescricaoProdutoVo.Criar(descricao);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComDescricaoVazia_DeveRetornarErro(string descricao)
    {
        var result = DescricaoProdutoVo.Criar(descricao);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("A descrição não pode ser vazia.");
    }

    [Fact]
    public void Criar_ComDescricaoMuitoLonga_DeveRetornarErro()
    {
        var descricao = new string('a', 101);

        var result = DescricaoProdutoVo.Criar(descricao);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("A descrição não pode ter mais de 100 caracteres.");
    }
}
