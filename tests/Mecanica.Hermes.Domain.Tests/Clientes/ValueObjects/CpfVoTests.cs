using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Clientes.ValueObjects;

public class CpfVoTests
{
    [Theory]
    [InlineData("12345678909")]
    [InlineData("123.456.789-09")]
    [InlineData("  123.456.789-09  ")]
    public void Criar_Should_ReturnSuccess_When_CpfIsValid(string cpf)
    {
        var result = CpfVo.Criar(cpf);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be("12345678909");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_Should_ReturnError_When_CpfIsEmpty(string cpf)
    {
        var result = CpfVo.Criar(cpf);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CPF não pode ser vazio.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345678")]
    public void Criar_Should_ReturnError_When_CpfHasInvalidFormat(string cpf)
    {
        var result = CpfVo.Criar(cpf);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CPF não está no formato correto.");
    }

    [Fact]
    public void Criar_Should_ReturnError_When_CpfHasTooManyDigits()
    {
        var result = CpfVo.Criar("123456789012345");

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CPF não está no formato correto.");
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("12345678901")]
    public void Criar_Should_ReturnError_When_CpfHasInvalidVerificationDigit(string cpf)
    {
        var result = CpfVo.Criar(cpf);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("CPF tem valor inválido.");
    }

    [Fact]
    public void ToString_Should_ReturnStoredValue()
    {
        var cpf = CpfVo.Criar("12345678909").Data!;

        cpf.ToString().Should().Be("12345678909");
    }
}
