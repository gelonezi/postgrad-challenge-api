using Mecanica.Hermes.Application.Clientes.Commands.AddCliente;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Application.Tests.Clientes.Commands;

public class AddClienteCommandHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IClienteMetrics> _metricsMock;
    private readonly AddClienteCommandHandler _handler;

    public AddClienteCommandHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _metricsMock = new Mock<IClienteMetrics>();
        _handler = new AddClienteCommandHandler(_repositoryMock.Object, _uowMock.Object, _metricsMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnCreatedCliente_When_DataIsValid()
    {
        var command = new AddClienteCommand("João Silva", null, "12345678909", "joao@example.com", "11987654321");

        _repositoryMock.Setup(r => r.GetByIdentificacaoFiscalAsync(It.IsAny<IdentificacaoFiscalVo>()))
            .ReturnsAsync((Cliente?)null);
        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<EmailVo>()))
            .ReturnsAsync((Cliente?)null);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.NomeCivil.Should().Be("João Silva");
        result.Data.Email.Should().Be("joao@example.com");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_DomainValidationFails()
    {
        var command = new AddClienteCommand("", null, "invalid", "invalid", "123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnConflict_When_IdentificacaoFiscalAlreadyExists()
    {
        var command = new AddClienteCommand("João Silva", null, "12345678909", "joao@example.com", "11987654321");
        var existingCliente = Cliente.Criar("Maria Santos", null, "12345678909", "maria@example.com", "11999887766").Data!;

        _repositoryMock.Setup(r => r.GetByIdentificacaoFiscalAsync(It.IsAny<IdentificacaoFiscalVo>()))
            .ReturnsAsync(existingCliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Identificação fiscal já cadastrada.");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnConflict_When_EmailAlreadyExists()
    {
        var command = new AddClienteCommand("João Silva", null, "12345678909", "joao@example.com", "11987654321");
        var existingCliente = Cliente.Criar("Maria Santos", null, "98765432100", "joao@example.com", "11999887766").Data!;

        _repositoryMock.Setup(r => r.GetByIdentificacaoFiscalAsync(It.IsAny<IdentificacaoFiscalVo>()))
            .ReturnsAsync((Cliente?)null);
        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<EmailVo>()))
            .ReturnsAsync(existingCliente);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().Contain("Email já cadastrado.");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Cliente>()), Times.Never);
    }
}
