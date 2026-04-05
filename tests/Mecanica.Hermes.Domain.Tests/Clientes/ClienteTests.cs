using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Domain.Tests.Clientes;

public class ClienteTests
{
    [Fact]
    public void Criar_Should_ReturnClienteWithDomainEvent_When_DataIsValid()
    {
        var result = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321");

        result.IsSuccess.Should().BeTrue();
        var cliente = result.Data!;
        cliente.NomeCivil.Valor.Should().Be("João Silva");
        cliente.NomeSocial.Should().BeNull();
        cliente.IdentificacaoFiscal.Valor.Should().Be("12345678909");
        cliente.Email.Valor.Should().Be("joao@example.com");
        cliente.Telefone.Valor.Should().Be("11987654321");
        cliente.Veiculos.Should().BeEmpty();
        cliente.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void Criar_Should_ReturnClienteWithNomeSocialNormalized_When_NomeSocialIsProvided()
    {
        var result = Cliente.Criar("João Silva", "João da Silva", "12345678909", "joao@example.com", "11987654321");

        result.IsSuccess.Should().BeTrue();
        result.Data!.NomeSocial!.Valor.Should().Be("João da Silva");
    }

    [Theory]
    [InlineData("", null, "12345678909", "joao@example.com", "11987654321")]
    [InlineData("João Silva", null, "invalid", "joao@example.com", "11987654321")]
    [InlineData("João Silva", null, "12345678909", "invalid-email", "11987654321")]
    [InlineData("João Silva", null, "12345678909", "joao@example.com", "123")]
    public void Criar_Should_ReturnFailure_When_DataIsInvalid(
        string nomeCivil, string? nomeSocial, string identificacaoFiscal, string email, string telefone)
    {
        var result = Cliente.Criar(nomeCivil, nomeSocial, identificacaoFiscal, email, telefone);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void AtualizarDados_Should_ModifyClienteAndAddEvent_When_DataIsValid()
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AtualizarDados("Maria Santos", null, "maria@example.com", "11999887766");

        result.IsSuccess.Should().BeTrue();
        cliente.NomeCivil.Valor.Should().Be("Maria Santos");
        cliente.NomeSocial.Should().BeNull();
        cliente.Email.Valor.Should().Be("maria@example.com");
        cliente.Telefone.Valor.Should().Be("11999887766");
        cliente.UpdatedAt.Should().NotBeNull();
        cliente.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void AtualizarDados_Should_ReturnFailure_When_DataIsInvalid()
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AtualizarDados("", null, "invalid", "123");

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void AdicionarVeiculo_Should_AddVeiculoAndRaiseEvent_When_DataIsValid()
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        cliente.Veiculos.Should().HaveCount(1);
        cliente.Veiculos.First().Modelo.Should().Be("Civic");
        cliente.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void AdicionarVeiculo_Should_AddSecondVeiculo_When_PlacaIsDifferent()
    {
        var cliente = CriarClientePadrao();
        cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);

        var result = cliente.AdicionarVeiculo("Corolla", "Toyota", "DEF5678", 2021);

        result.IsSuccess.Should().BeTrue();
        cliente.Veiculos.Should().HaveCount(2);
    }

    [Fact]
    public void AlterarVeiculo_Should_UpdateVeiculoAndRaiseEvent_When_VeiculoExists()
    {
        var cliente = CriarClientePadrao();
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;

        var result = cliente.AlterarVeiculo(veiculoId, "Civic EX", "Honda", 2021);

        result.IsSuccess.Should().BeTrue();
        var veiculo = cliente.Veiculos.First();
        veiculo.Modelo.Should().Be("Civic EX");
        veiculo.Ano.Should().Be(2021);
        cliente.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public void AlterarVeiculo_Should_ReturnFailure_When_VeiculoDoesNotExist()
    {
        var cliente = CriarClientePadrao();

        var result = cliente.AlterarVeiculo(Guid.NewGuid(), "Civic", "Honda", 2020);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Veículo não encontrado.");
    }

    [Fact]
    public void RemoverVeiculo_Should_SoftDeleteVeiculoAndRaiseEvent_When_VeiculoExists()
    {
        var cliente = CriarClientePadrao();
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;

        var result = cliente.RemoverVeiculo(veiculoId);

        result.IsSuccess.Should().BeTrue();
        cliente.Veiculos.Should().HaveCount(1);
        cliente.Veiculos.First().IsDeleted.Should().BeTrue();
        cliente.DomainEvents.Should().HaveCount(3);
    }

    [Fact]
    public void RemoverVeiculo_Should_ReturnNotFound_When_VeiculoDoesNotExist()
    {
        var cliente = CriarClientePadrao();

        var result = cliente.RemoverVeiculo(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Excluir_Should_SoftDeleteClienteAndRaiseEvent_When_ClienteIsActive()
    {
        var cliente = CriarClientePadrao();

        var result = cliente.Excluir();

        result.IsSuccess.Should().BeTrue();
        cliente.IsDeleted.Should().BeTrue();
        cliente.UpdatedAt.Should().NotBeNull();
        cliente.DomainEvents.Should().HaveCount(2);
    }

    [Fact]
    public void Excluir_Should_ReturnFailure_When_ClienteIsAlreadyDeleted()
    {
        var cliente = CriarClientePadrao();
        cliente.Excluir();

        var result = cliente.Excluir();

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Cliente já foi excluído.");
    }

    private static Cliente CriarClientePadrao()
    {
        return Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
    }
}
