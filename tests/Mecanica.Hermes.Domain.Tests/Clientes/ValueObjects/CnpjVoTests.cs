using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Clientes.ValueObjects;

public class CnpjVoTests
{
    [Theory]
    [InlineData("11222333000181")]
    public void Criar_Should_ReturnSuccess_When_CnpjIsValid(string cnpj)
    {
        var result = CnpjVo.Criar(cnpj);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be("11222333000181");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_Should_ReturnError_When_CnpjIsEmpty(string cnpj)
    {
        var result = CnpjVo.Criar(cnpj);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CNPJ não pode ser vazio.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("1122233300018")]
    [InlineData("112223330001811")]
    public void Criar_Should_ReturnError_When_CnpjHasInvalidFormat(string cnpj)
    {
        var result = CnpjVo.Criar(cnpj);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CNPJ não está no formato correto.");
    }

    [Theory]
    [InlineData("11222333000100")]
    [InlineData("11222333000199")]
    public void Criar_Should_ReturnError_When_CnpjHasInvalidVerificationDigit(string cnpj)
    {
        var result = CnpjVo.Criar(cnpj);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CNPJ tem valor inválido.");
    }

    [Fact]
    public void ToString_Should_ReturnStoredValue()
    {
        var cnpj = CnpjVo.Criar("11222333000181").Data!;

        cnpj.ToString().Should().Be("11222333000181");
    }
}
