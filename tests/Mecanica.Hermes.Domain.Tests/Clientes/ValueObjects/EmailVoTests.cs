using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Clientes.ValueObjects;

public class EmailVoTests
{
    [Theory]
    [InlineData("teste@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("TESTE@EXAMPLE.COM")]
    public void Criar_Should_ReturnSuccess_When_EmailIsValid(string email)
    {
        var result = EmailVo.Criar(email);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be(email.ToLowerInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_Should_ReturnError_When_EmailIsEmpty(string email)
    {
        var result = EmailVo.Criar(email);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Email é obrigatório.");
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData("user name@example.com")]
    public void Criar_Should_ReturnError_When_EmailHasInvalidFormat(string email)
    {
        var result = EmailVo.Criar(email);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Email em formato inválido.");
    }

    [Fact]
    public void Criar_Should_ReturnError_When_EmailExceedsMaxLength()
    {
        var email = new string('a', 191) + "@example.com";

        var result = EmailVo.Criar(email);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Email não pode ter mais do que 200 caracteres.");
    }
}
