using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Clientes.Queries.ListClientesByNome;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Common.Pagination;

namespace Mecanica.Hermes.Application.Tests.Clientes.Queries;

public class ListClientesByNomeQueryHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ListClientesByNomeQueryHandler _handler;

    public ListClientesByNomeQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _handler = new ListClientesByNomeQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedResult_When_ClientesExist()
    {
        var clientes = new List<Cliente>
        {
            Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!,
            Cliente.Criar("João Santos", null, "98765432100", "joao2@example.com", "11999887766").Data!
        };
        var pagedClientes = PagedResult<Cliente>.Create(clientes, 1, 10, 2);
        var query = new ListClientesByNomeQuery("João", 1, 10);

        _repositoryMock.Setup(r => r.ListByNomeAsync("João", It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedClientes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyPagedResult_When_NoClientesMatch()
    {
        var pagedClientes = PagedResult<Cliente>.Create([], 1, 10, 0);
        var query = new ListClientesByNomeQuery("Inexistente", 1, 10);

        _repositoryMock.Setup(r => r.ListByNomeAsync("Inexistente", It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedClientes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_PassCorrectPaginationParams_When_QueryHasCustomPage()
    {
        var pagedClientes = PagedResult<Cliente>.Create([], 2, 5, 0);
        var query = new ListClientesByNomeQuery("João", 2, 5);

        _repositoryMock.Setup(r => r.ListByNomeAsync("João", It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)))
            .ReturnsAsync(pagedClientes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.ListByNomeAsync("João", It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)), Times.Once);
    }
}
