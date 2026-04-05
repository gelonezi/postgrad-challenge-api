using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Clientes.ValueObjects;

public class TelefoneVoTests
{
    [Theory]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("  (11) 98765-4321  ")]
    public void Criar_Should_ReturnSuccess_When_TelefoneIsValid(string telefone)
    {
        var result = TelefoneVo.Criar(telefone);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be("11987654321");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_Should_ReturnError_When_TelefoneIsEmpty(string telefone)
    {
        var result = TelefoneVo.Criar(telefone);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Telefone é obrigatório.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("119876543")]
    [InlineData("119876543210")]
    public void Criar_Should_ReturnError_When_TelefoneHasInvalidLength(string telefone)
    {
        var result = TelefoneVo.Criar(telefone);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Telefone deve conter DDD e número válido.");
    }

    [Fact]
    public void ToString_Should_ReturnStoredValue()
    {
        var telefone = TelefoneVo.Criar("11987654321").Data!;

        telefone.ToString().Should().Be("11987654321");
    }
}
