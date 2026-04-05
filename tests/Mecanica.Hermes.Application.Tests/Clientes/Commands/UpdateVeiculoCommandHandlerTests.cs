using Mecanica.Hermes.Application.Clientes.Commands.UpdateVeiculo;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Application.Tests.Clientes.Commands;

public class UpdateVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly UpdateVeiculoCommandHandler _handler;

    public UpdateVeiculoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new UpdateVeiculoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_UpdateVeiculo_When_ClienteAndVeiculoExist()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;
        var command = new UpdateVeiculoCommand(clienteId, veiculoId, "Civic EX", "Honda", 2022);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Veiculos.First().Modelo.Should().Be("Civic EX");
        result.Data.Veiculos.First().Ano.Should().Be(2022);
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ClienteDoesNotExist()
    {
        var command = new UpdateVeiculoCommand(Guid.NewGuid(), Guid.NewGuid(), "Civic EX", "Honda", 2022);

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_VeiculoDoesNotExist()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var command = new UpdateVeiculoCommand(clienteId, Guid.NewGuid(), "Civic EX", "Honda", 2022);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Veículo não encontrado.");
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_VeiculoDataIsInvalid()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;
        var command = new UpdateVeiculoCommand(clienteId, veiculoId, "", "Honda", 1800);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
