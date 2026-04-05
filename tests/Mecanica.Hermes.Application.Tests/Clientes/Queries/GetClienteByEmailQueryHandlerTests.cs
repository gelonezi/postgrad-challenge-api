using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Clientes.Queries.GetClienteByEmail;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Application.Tests.Clientes.Queries;

public class GetClienteByEmailQueryHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly GetClienteByEmailQueryHandler _handler;

    public GetClienteByEmailQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _handler = new GetClienteByEmailQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnClienteDto_When_EmailExists()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var query = new GetClienteByEmailQuery("joao@example.com");

        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<EmailVo>())).ReturnsAsync(cliente);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be("joao@example.com");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_EmailDoesNotExist()
    {
        var query = new GetClienteByEmailQuery("naoexiste@example.com");

        _repositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<EmailVo>())).ReturnsAsync((Cliente?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_EmailIsInvalid()
    {
        var query = new GetClienteByEmailQuery("invalid-email");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.GetByEmailAsync(It.IsAny<EmailVo>()), Times.Never);
    }
}
