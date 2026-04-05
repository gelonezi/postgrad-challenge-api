using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Clientes.ValueObjects;

public class PlacaVoTests
{
    [Theory]
    [InlineData("ABC1234")]
    [InlineData("abc1234")]
    [InlineData("  ABC1234  ")]
    public void Criar_Should_ReturnSuccess_When_PlacaIsOldFormat(string placa)
    {
        var result = PlacaVo.Criar(placa);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be("ABC1234");
    }

    [Theory]
    [InlineData("ABC1D23")]
    [InlineData("abc1d23")]
    [InlineData("  ABC1D23  ")]
    public void Criar_Should_ReturnSuccess_When_PlacaIsMercosulFormat(string placa)
    {
        var result = PlacaVo.Criar(placa);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be("ABC1D23");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_Should_ReturnError_When_PlacaIsEmpty(string placa)
    {
        var result = PlacaVo.Criar(placa);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("A placa não pode ser vazia.");
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("ABC12345")]
    [InlineData("A1B2C3D")]
    [InlineData("1234567")]
    [InlineData("ABCDEFG")]
    public void Criar_Should_ReturnError_When_PlacaHasInvalidFormat(string placa)
    {
        var result = PlacaVo.Criar(placa);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Placa inválida. Formatos aceitos: ABC1234 ou ABC1D23.");
    }
}
