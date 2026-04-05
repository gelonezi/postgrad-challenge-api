using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Application.Produtos.Queries.ListAllServicos;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Queries;

public class ListAllServicosQueryHandlerTests
{
    private readonly Mock<IServicoRepository> _repositoryMock;
    private readonly ListAllServicosQueryHandler _handler;

    public ListAllServicosQueryHandlerTests()
    {
        _repositoryMock = new Mock<IServicoRepository>();
        _handler = new ListAllServicosQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedResult_When_ServicosExist()
    {
        var servicos = new List<Servico>
        {
            Servico.Criar("Troca de Óleo", 150.00m).Data!,
            Servico.Criar("Alinhamento", 100.00m).Data!,
            Servico.Criar("Balanceamento", 80.00m).Data!
        };
        var pagedServicos = PagedResult<Servico>.Create(servicos, 1, 10, 3);
        var query = new ListAllServicosQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedServicos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3);
        result.Data.TotalCount.Should().Be(3);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyPagedResult_When_NoServicosExist()
    {
        var pagedServicos = PagedResult<Servico>.Create([], 1, 10, 0);
        var query = new ListAllServicosQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedServicos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_PassCorrectPaginationParams_When_QueryHasCustomPage()
    {
        var pagedServicos = PagedResult<Servico>.Create([], 2, 5, 0);
        var query = new ListAllServicosQuery(2, 5);

        _repositoryMock.Setup(r => r.ListAllAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)))
            .ReturnsAsync(pagedServicos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.ListAllAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnServicosOrderedByDescricao_When_RepositoryReturnsOrderedData()
    {
        var servicos = new List<Servico>
        {
            Servico.Criar("Alinhamento", 100.00m).Data!,
            Servico.Criar("Balanceamento", 80.00m).Data!,
            Servico.Criar("Troca de Óleo", 150.00m).Data!
        };
        var pagedServicos = PagedResult<Servico>.Create(servicos, 1, 10, 3);
        var query = new ListAllServicosQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedServicos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var servicosList = result.Data!.Items.ToList();
        servicosList[0].Descricao.Should().Be("Alinhamento");
        servicosList[1].Descricao.Should().Be("Balanceamento");
        servicosList[2].Descricao.Should().Be("Troca de Óleo");
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectPageSize_When_PageSizeIsSmall()
    {
        var servicos = new List<Servico>
        {
            Servico.Criar("Alinhamento", 100.00m).Data!,
            Servico.Criar("Balanceamento", 80.00m).Data!
        };
        var pagedServicos = PagedResult<Servico>.Create(servicos, 1, 2, 5);
        var query = new ListAllServicosQuery(1, 2);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedServicos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(5);
        result.Data.PageSize.Should().Be(2);
    }
}
