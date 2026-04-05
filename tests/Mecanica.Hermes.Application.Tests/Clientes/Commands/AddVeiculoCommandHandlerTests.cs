using Mecanica.Hermes.Application.Clientes.Commands.AddVeiculo;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Application.Tests.Clientes.Commands;

public class AddVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly AddVeiculoCommandHandler _handler;

    public AddVeiculoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new AddVeiculoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_AddVeiculoToCliente_When_ClienteExistsAndDataIsValid()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var command = new AddVeiculoCommand(clienteId, "Civic", "Honda", "ABC1234", 2020);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Veiculos.Should().HaveCount(1);
        result.Data.Veiculos.First().Modelo.Should().Be("Civic");
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ClienteDoesNotExist()
    {
        var command = new AddVeiculoCommand(Guid.NewGuid(), "Civic", "Honda", "ABC1234", 2020);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_PlacaIsInvalid()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var command = new AddVeiculoCommand(clienteId, "Civic", "Honda", "INVALID", 2020);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_AddVeiculo_When_ClienteHasNoVeiculos()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var command = new AddVeiculoCommand(clienteId, "Corolla", "Toyota", "DEF5678", 2021);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Veiculos.Should().HaveCount(1);
        result.Data.Veiculos[0].Placa.Should().Be("DEF5678");
    }
}
