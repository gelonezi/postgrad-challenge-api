using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Commands.DeleteServico;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Commands;

public class DeleteServicoCommandHandlerTests
{
    private readonly Mock<IServicoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly DeleteServicoCommandHandler _handler;

    public DeleteServicoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IServicoRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new DeleteServicoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ServicoDoesNotExist()
    {
        var command = new DeleteServicoCommand(Guid.NewGuid());
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync((Servico?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_DeleteServico_When_ServicoExists()
    {
        var servico = Servico.Criar("Troca de Óleo", 80.00m).Data!;
        var command = new DeleteServicoCommand(servico.Id);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync(servico);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        servico.IsDeleted.Should().BeTrue();
        _repositoryMock.Verify(x => x.Update(servico), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
