using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Commands.DeleteProduto;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Commands;

public class DeleteProdutoCommandHandlerTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly DeleteProdutoCommandHandler _handler;

    public DeleteProdutoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new DeleteProdutoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ProdutoDoesNotExist()
    {
        var command = new DeleteProdutoCommand(Guid.NewGuid());
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync((Produto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_DeleteProduto_When_ProdutoExists()
    {
        var produto = Produto.Criar("Filtro de Óleo", 45.00m, 10, "Peca").Data!;
        var command = new DeleteProdutoCommand(produto.Id);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync(produto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        produto.IsDeleted.Should().BeTrue();
        _repositoryMock.Verify(x => x.Update(produto), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
