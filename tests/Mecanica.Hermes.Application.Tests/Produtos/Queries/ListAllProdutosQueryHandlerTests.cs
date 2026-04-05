using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Application.Produtos.Queries.ListAllProdutos;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Queries;

public class ListAllProdutosQueryHandlerTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly ListAllProdutosQueryHandler _handler;

    public ListAllProdutosQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        _handler = new ListAllProdutosQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPagedResult_When_ProdutosExist()
    {
        var produtos = new List<Produto>
        {
            Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca").Data!,
            Produto.Criar("Filtro de Ar", 30.00m, 15, "Peca").Data!,
            Produto.Criar("Pastilha de Freio", 80.00m, 8, "Peca").Data!
        };
        var pagedProdutos = PagedResult<Produto>.Create(produtos, 1, 10, 3);
        var query = new ListAllProdutosQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedProdutos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(3);
        result.Data.TotalCount.Should().Be(3);
        result.Data.Page.Should().Be(1);
        result.Data.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyPagedResult_When_NoProdutosExist()
    {
        var pagedProdutos = PagedResult<Produto>.Create([], 1, 10, 0);
        var query = new ListAllProdutosQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedProdutos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_Should_PassCorrectPaginationParams_When_QueryHasCustomPage()
    {
        var pagedProdutos = PagedResult<Produto>.Create([], 2, 5, 0);
        var query = new ListAllProdutosQuery(2, 5);

        _repositoryMock.Setup(r => r.ListAllAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)))
            .ReturnsAsync(pagedProdutos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repositoryMock.Verify(r => r.ListAllAsync(It.Is<PaginationParams>(p => p.Page == 2 && p.PageSize == 5)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnProdutosOrderedByDescricao_When_RepositoryReturnsOrderedData()
    {
        var produtos = new List<Produto>
        {
            Produto.Criar("Filtro de Ar", 30.00m, 15, "Peca").Data!,
            Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca").Data!,
            Produto.Criar("Pastilha de Freio", 80.00m, 8, "Peca").Data!
        };
        var pagedProdutos = PagedResult<Produto>.Create(produtos, 1, 10, 3);
        var query = new ListAllProdutosQuery(1, 10);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedProdutos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var produtosList = result.Data!.Items.ToList();
        produtosList[0].Descricao.Should().Be("Filtro de Ar");
        produtosList[1].Descricao.Should().Be("Óleo de Motor");
        produtosList[2].Descricao.Should().Be("Pastilha de Freio");
    }

    [Fact]
    public async Task Handle_Should_ReturnCorrectPageSize_When_PageSizeIsSmall()
    {
        var produtos = new List<Produto>
        {
            Produto.Criar("Filtro de Ar", 30.00m, 15, "Peca").Data!,
            Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca").Data!
        };
        var pagedProdutos = PagedResult<Produto>.Create(produtos, 1, 2, 5);
        var query = new ListAllProdutosQuery(1, 2);

        _repositoryMock.Setup(r => r.ListAllAsync(It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedProdutos);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(5);
        result.Data.PageSize.Should().Be(2);
    }
}
