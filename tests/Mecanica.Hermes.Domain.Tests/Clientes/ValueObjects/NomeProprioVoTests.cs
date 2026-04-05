using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Domain.Tests.Clientes.ValueObjects;

public class NomeProprioVoTests
{
    [Theory]
    [InlineData("joão silva", "João Silva")]
    [InlineData("MARIA SANTOS", "Maria Santos")]
    [InlineData("  pedro  oliveira  ", "Pedro Oliveira")]
    public void Criar_Should_ReturnNormalizedSuccess_When_NomeIsValid(string nome, string esperado)
    {
        var result = NomeProprioVo.Criar(nome);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_Should_ReturnError_When_NomeIsEmpty(string nome)
    {
        var result = NomeProprioVo.Criar(nome);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Nome não pode ser vazio.");
    }

    [Fact]
    public void Criar_Should_ReturnError_When_NomeExceedsMaxLength()
    {
        var nome = new string('a', 101) + " " + new string('b', 101);

        var result = NomeProprioVo.Criar(nome);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Nome não pode ter mais de 200 caracteres.");
    }

    [Fact]
    public void Criar_Should_ReturnError_When_NomeHasOnlyOneWord()
    {
        var result = NomeProprioVo.Criar("João");

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Nome deve conter pelo menos nome e sobrenome.");
    }

    [Theory]
    [InlineData("joão da silva", "João da Silva")]
    [InlineData("maria de souza", "Maria de Souza")]
    [InlineData("pedro dos santos", "Pedro dos Santos")]
    public void Criar_Should_KeepPrepositionsLowercase_When_NomeContainsPrepositions(string nome, string esperado)
    {
        var result = NomeProprioVo.Criar(nome);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Valor.Should().Be(esperado);
    }
}
