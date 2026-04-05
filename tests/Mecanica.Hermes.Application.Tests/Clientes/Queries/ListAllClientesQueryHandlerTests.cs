using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Clientes.Queries.ListAllClientes;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Common.Pagination;

namespace Mecanica.Hermes.Application.Tests.Clientes.Queries;

public class ListAllClientesQueryHandlerTests
{
    private readonly Mock<IClienteRepository> _repositoryMock;
    private readonly ListAllClientesQueryHandler _handler;

    public ListAllClientesQueryHandlerTests()
    {
        _repositoryMock = new Mock<IClienteRepository>();
        _handler = new ListAllClientesQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedResult_When_ClientesExist()
    {
        var clientes = new List<Cliente>
        {
            Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!,
            Cliente.Criar("Maria Santos", null, "12345678909", "maria@example.com", "11999887766").Data!,
            Cliente.Criar("Pedro Oliveira", null, "12345678909", "pedro@example.com", "11988776655").Data!
        };
        var pagedClientes = PagedResult<Cliente>.Create(clientes, 1, 10, 3);
        var query = new ListAllClientesQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedClientes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3);
        result.Data.TotalCount.Should().Be(3);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyPagedResult_When_NoClientesExist()
    {
        var pagedClientes = PagedResult<Cliente>.Create([], 1, 10, 0);
        var query = new ListAllClientesQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
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
        var query = new ListAllClientesQuery(2, 5);

        _repositoryMock.Setup(r => r.ListAllAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)))
            .ReturnsAsync(pagedClientes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.ListAllAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedResultWithCorrectDtos_When_ClientesHaveVeiculos()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        
        var clientes = new List<Cliente> { cliente };
        var pagedClientes = PagedResult<Cliente>.Create(clientes, 1, 10, 1);
        var query = new ListAllClientesQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedClientes);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        var clienteDto = result.Data.Items.First();
        clienteDto.Veiculos.Should().HaveCount(1);
        clienteDto.Veiculos[0].Placa.Should().Be("ABC1234");
    }
}
