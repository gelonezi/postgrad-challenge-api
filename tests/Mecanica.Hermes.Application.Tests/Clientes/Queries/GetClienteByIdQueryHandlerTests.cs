using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Clientes.Queries.GetClienteById;
using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Application.Tests.Clientes.Queries;

public class GetClienteByIdQueryHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly GetClienteByIdQueryHandler _handler;

    public GetClienteByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _handler = new GetClienteByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnClienteDto_When_ClienteExists()
    {
        var clienteId = Guid.NewGuid();
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var query = new GetClienteByIdQuery(clienteId);

        _repositoryMock.Setup(r => r.GetByIdAsync(clienteId)).ReturnsAsync(cliente);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.NomeCivil.Should().Be("João Silva");
        result.Data.Email.Should().Be("joao@example.com");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ClienteDoesNotExist()
    {
        var query = new GetClienteByIdQuery(Guid.NewGuid());

        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
