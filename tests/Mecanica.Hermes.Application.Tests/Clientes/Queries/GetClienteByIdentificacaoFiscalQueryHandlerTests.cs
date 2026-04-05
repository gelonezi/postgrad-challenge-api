using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Clientes.Queries.GetClienteByIdentificacaoFiscal;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Application.Tests.Clientes.Queries;

public class GetClienteByIdentificacaoFiscalQueryHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly GetClienteByIdentificacaoFiscalQueryHandler _handler;

    public GetClienteByIdentificacaoFiscalQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _handler = new GetClienteByIdentificacaoFiscalQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnClienteDto_When_IdentificacaoFiscalExists()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var query = new GetClienteByIdentificacaoFiscalQuery("12345678909");

        _repositoryMock.Setup(r => r.GetByIdentificacaoFiscalAsync(It.IsAny<IdentificacaoFiscalVo>()))
            .ReturnsAsync(cliente);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.IdentificacaoFiscal.Should().Be("12345678909");
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_IdentificacaoFiscalDoesNotExist()
    {
        var query = new GetClienteByIdentificacaoFiscalQuery("12345678909");

        _repositoryMock.Setup(r => r.GetByIdentificacaoFiscalAsync(It.IsAny<IdentificacaoFiscalVo>()))
            .ReturnsAsync((Cliente?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_IdentificacaoFiscalIsInvalid()
    {
        var query = new GetClienteByIdentificacaoFiscalQuery("invalid");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        _repositoryMock.Verify(r => r.GetByIdentificacaoFiscalAsync(It.IsAny<IdentificacaoFiscalVo>()), Times.Never);
    }
}
