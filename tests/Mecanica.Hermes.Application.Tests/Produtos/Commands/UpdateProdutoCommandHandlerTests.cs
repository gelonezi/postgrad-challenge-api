using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Commands.UpdateProduto;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Commands;

public class UpdateProdutoCommandHandlerTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly UpdateProdutoCommandHandler _handler;

    public UpdateProdutoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new UpdateProdutoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ProdutoDoesNotExist()
    {
        var command = new UpdateProdutoCommand(Guid.NewGuid(), "Nova Descrição", 50.00m);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync((Produto?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateProduto_When_ProdutoExists()
    {
        var produto = Produto.Criar("Filtro de Óleo", 45.00m, 10, "Peca").Data!;
        var command = new UpdateProdutoCommand(produto.Id, "Filtro de Óleo Premium", 55.00m);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync(produto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Should().Be("Filtro de Óleo Premium");
        result.Data.Valor.Should().Be(55.00m);
        _repositoryMock.Verify(x => x.Update(produto), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_DataIsInvalid()
    {
        var produto = Produto.Criar("Filtro de Óleo", 45.00m, 10, "Peca").Data!;
        var command = new UpdateProdutoCommand(produto.Id, "", -10.00m);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync(produto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
