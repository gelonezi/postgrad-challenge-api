using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Application.Produtos.Queries.ListProdutosByDescricao;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Application.Tests.Produtos.Queries;

public class ListProdutosByDescricaoQueryHandlerTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly ListProdutosByDescricaoQueryHandler _handler;

    public ListProdutosByDescricaoQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        _handler = new ListProdutosByDescricaoQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedProdutos()
    {
        var produtos = new List<Produto>
        {
            Produto.Criar("Filtro de Óleo", 45.00m, 10, "Peca").Data!,
            Produto.Criar("Filtro de Ar", 35.00m, 15, "Peca").Data!
        };

        var pagedResult = PagedResult<Produto>.Create(produtos, 1, 10, 2);
        var query = new ListProdutosByDescricaoQuery("Filtro", 1, 10);
        
        _repositoryMock
            .Setup(x => x.ListByDescricaoAsync(It.IsAny<DescricaoProdutoVo>(), It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_NoProdutosFound()
    {
        var pagedResult = PagedResult<Produto>.Create(new List<Produto>(), 1, 10, 0);
        var query = new ListProdutosByDescricaoQuery("Inexistente", 1, 10);
        
        _repositoryMock
            .Setup(x => x.ListByDescricaoAsync(It.IsAny<DescricaoProdutoVo>(), It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }
}
