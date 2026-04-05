using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.OrdensDeServico.Queries.ListOrdemDeServicoByStatus;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.OrdensDeServico;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Queries;

public class ListOrdemDeServicoByStatusQueryHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly ListOrdemDeServicoByStatusQueryHandler _handler;

    public ListOrdemDeServicoByStatusQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _handler = new ListOrdemDeServicoByStatusQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaPaginada_QuandoStatusValido()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;

        var ordens = new List<OrdemDeServico>
        {
            OrdemDeServico.Criar(cliente, veiculo, "Problema 1", null).Data!,
            OrdemDeServico.Criar(cliente, veiculo, "Problema 2", null).Data!
        };

        var pagedResult = PagedResult<OrdemDeServico>.Create(ordens, 1, 10, 2);
        _repositoryMock.Setup(r => r.ListByStatusAsync(It.IsAny<Domain.OrdensDeServico.Enums.OrdemDeServicoStatus>(), It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        var query = new ListOrdemDeServicoByStatusQuery("EmDiagnostico", 1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Items.Count());
        Assert.Equal(2, result.Data.TotalCount);
        _repositoryMock.Verify(r => r.ListByStatusAsync(It.IsAny<Domain.OrdensDeServico.Enums.OrdemDeServicoStatus>(), It.IsAny<PaginationParams>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarBadRequest_QuandoStatusInvalido()
    {
        var query = new ListOrdemDeServicoByStatusQuery("StatusInexistente", 1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        _repositoryMock.Verify(r => r.ListByStatusAsync(It.IsAny<Domain.OrdensDeServico.Enums.OrdemDeServicoStatus>(), It.IsAny<PaginationParams>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarListaVazia_QuandoNaoExistemOrdensComOStatus()
    {
        var pagedResult = PagedResult<OrdemDeServico>.Create(new List<OrdemDeServico>(), 1, 10, 0);
        _repositoryMock.Setup(r => r.ListByStatusAsync(It.IsAny<Domain.OrdensDeServico.Enums.OrdemDeServicoStatus>(), It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        var query = new ListOrdemDeServicoByStatusQuery("EmDiagnostico", 1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Data!.Items);
        Assert.Equal(0, result.Data.TotalCount);
    }
}
