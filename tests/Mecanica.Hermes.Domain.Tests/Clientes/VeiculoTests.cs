using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Domain.Tests.Clientes;

public class VeiculoTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarVeiculo_Should_ReturnError_When_ModeloIsEmpty(string modelo)
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AdicionarVeiculo(modelo, "Honda", "ABC1234", 2020);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Modelo é obrigatório.");
    }

    [Fact]
    public void AdicionarVeiculo_Should_ReturnError_When_ModeloExceedsMaxLength()
    {
        var cliente = CriarClientePadrao();
        var modeloLongo = new string('a', 101);

        var result = cliente.AdicionarVeiculo(modeloLongo, "Honda", "ABC1234", 2020);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Modelo não pode ter mais de 100 caracteres.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AdicionarVeiculo_Should_ReturnError_When_MarcaIsEmpty(string marca)
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AdicionarVeiculo("Civic", marca, "ABC1234", 2020);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Marca é obrigatória.");
    }

    [Fact]
    public void AdicionarVeiculo_Should_ReturnError_When_MarcaExceedsMaxLength()
    {
        var cliente = CriarClientePadrao();
        var marcaLonga = new string('a', 101);

        var result = cliente.AdicionarVeiculo("Civic", marcaLonga, "ABC1234", 2020);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Marca não pode ter mais de 100 caracteres.");
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2101)]
    public void AdicionarVeiculo_Should_ReturnError_When_AnoIsOutOfRange(int ano)
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", ano);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Ano deve estar entre 1900 e 2100.");
    }

    [Theory]
    [InlineData(1900)]
    [InlineData(2100)]
    public void AdicionarVeiculo_Should_ReturnSuccess_When_AnoIsAtBoundary(int ano)
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", ano);

        result.IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AlterarVeiculo_Should_ReturnError_When_ModeloIsEmpty(string modelo)
    {
        var cliente = CriarClientePadrao();
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;

        var result = cliente.AlterarVeiculo(veiculoId, modelo, "Honda", 2021);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Modelo é obrigatório.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AlterarVeiculo_Should_ReturnError_When_MarcaIsEmpty(string marca)
    {
        var cliente = CriarClientePadrao();
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;

        var result = cliente.AlterarVeiculo(veiculoId, "Civic", marca, 2021);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Marca é obrigatória.");
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2101)]
    public void AlterarVeiculo_Should_ReturnError_When_AnoIsOutOfRange(int ano)
    {
        var cliente = CriarClientePadrao();
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;

        var result = cliente.AlterarVeiculo(veiculoId, "Civic", "Honda", ano);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Ano deve estar entre 1900 e 2100.");
    }

    private static Cliente CriarClientePadrao()
    {
        return Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
    }
}
