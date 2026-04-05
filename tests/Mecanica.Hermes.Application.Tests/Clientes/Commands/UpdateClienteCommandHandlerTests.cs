using Mecanica.Hermes.Application.Clientes.Commands.UpdateCliente;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Application.Tests.Clientes.Commands;

public class UpdateClienteCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly UpdateClienteCommandHandler _handler;

    public UpdateClienteCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new UpdateClienteCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnUpdatedCliente_When_ClienteExistsAndDataIsValid()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var command = new UpdateClienteCommand(clienteId, "Maria Santos", null, "maria@example.com", "11999887766");

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.NomeCivil.Should().Be("Maria Santos");
        result.Data.Email.Should().Be("maria@example.com");
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ClienteDoesNotExist()
    {
        var command = new UpdateClienteCommand(Guid.NewGuid(), "Maria Santos", null, "maria@example.com", "11999887766");

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_DomainValidationFails()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var command = new UpdateClienteCommand(clienteId, "", null, "invalid-email", "123");

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.Update(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
