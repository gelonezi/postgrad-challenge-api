using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Application.Produtos.Queries.GetProdutoById;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Queries;

public class GetProdutoByIdQueryHandlerTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly GetProdutoByIdQueryHandler _handler;

    public GetProdutoByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        _handler = new GetProdutoByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ProdutoDoesNotExist()
    {
        var query = new GetProdutoByIdQuery(Guid.NewGuid());
        _repositoryMock.Setup(x => x.GetByIdAsync(query.Id)).ReturnsAsync((Produto?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnProduto_When_ProdutoExists()
    {
        var produto = Produto.Criar("Filtro de Óleo", 45.00m, 10, "Peca").Data!;
        var query = new GetProdutoByIdQuery(produto.Id);
        _repositoryMock.Setup(x => x.GetByIdAsync(query.Id)).ReturnsAsync(produto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(produto.Id);
        result.Data.Descricao.Should().Be("Filtro de Óleo");
        result.Data.Valor.Should().Be(45.00m);
    }
}
