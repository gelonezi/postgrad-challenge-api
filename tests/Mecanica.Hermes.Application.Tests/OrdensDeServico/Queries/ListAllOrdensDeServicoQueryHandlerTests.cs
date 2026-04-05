using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.OrdensDeServico.Queries.ListAllOrdensDeServico;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.OrdensDeServico;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Queries;

public class ListAllOrdensDeServicoQueryHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly ListAllOrdensDeServicoQueryHandler _handler;

    public ListAllOrdensDeServicoQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _handler = new ListAllOrdensDeServicoQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaPaginada_ComSucesso()
    {
        var clienteResult = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999");
        var cliente = clienteResult.Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculo = veiculoResult.Data!;
        
        var ordens = new List<OrdemDeServico>
        {
            OrdemDeServico.Criar(cliente, veiculo, "Problema 1", null).Data!,
            OrdemDeServico.Criar(cliente, veiculo, "Problema 2", null).Data!
        };

        var pagedResult = PagedResult<OrdemDeServico>.Create(ordens, 1, 10, 2);
        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>())).ReturnsAsync(pagedResult);

        var query = new ListAllOrdensDeServicoQuery(1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Equal(2, result.Data.TotalCount);
        _repositoryMock.Verify(r => r.ListAllAsync(It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistemOrdens()
    {
        var pagedResult = PagedResult<OrdemDeServico>.Create(new List<OrdemDeServico>(), 1, 10, 0);
        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>())).ReturnsAsync(pagedResult);

        var query = new ListAllOrdensDeServicoQuery(1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }
}
