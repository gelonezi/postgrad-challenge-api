using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Commands.UpdateServico;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Commands;

public class UpdateServicoCommandHandlerTests
{
    private readonly Mock<IServicoRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly UpdateServicoCommandHandler _handler;

    public UpdateServicoCommandHandlerTests()
    {
        _repositoryMock = new Mock<IServicoRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _handler = new UpdateServicoCommandHandler(_repositoryMock.Object, _uowMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ServicoDoesNotExist()
    {
        var command = new UpdateServicoCommand(Guid.NewGuid(), "Nova Descrição", 100.00m);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync((Servico?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateServico_When_ServicoExists()
    {
        var servico = Servico.Criar("Troca de Óleo", 80.00m).Data!;
        var command = new UpdateServicoCommand(servico.Id, "Troca de Óleo Completa", 120.00m);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync(servico);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Should().Be("Troca de Óleo Completa");
        result.Data.Valor.Should().Be(120.00m);
        _repositoryMock.Verify(x => x.Update(servico), Times.Once);
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnBadRequest_When_DataIsInvalid()
    {
        var servico = Servico.Criar("Troca de Óleo", 80.00m).Data!;
        var command = new UpdateServicoCommand(servico.Id, "", -50.00m);
        _repositoryMock.Setup(x => x.GetByIdAsync(command.Id)).ReturnsAsync(servico);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        _uowMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
