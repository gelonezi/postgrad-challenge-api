using Mecanica.Hermes.Application.Clientes.Commands.DeleteVeiculo;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Application.Tests.Clientes.Commands;

public class DeleteVeiculoCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly DeleteVeiculoCommandHandler _handler;

    public DeleteVeiculoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new DeleteVeiculoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_SoftDeleteVeiculo_When_ClienteAndVeiculoExist()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculoId = veiculoResult.Data!.Id;
        var command = new DeleteVeiculoCommand(clienteId, veiculoId);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ClienteDoesNotExist()
    {
        var command = new DeleteVeiculoCommand(Guid.NewGuid(), Guid.NewGuid());

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
        var command = new DeleteVeiculoCommand(clienteId, Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
