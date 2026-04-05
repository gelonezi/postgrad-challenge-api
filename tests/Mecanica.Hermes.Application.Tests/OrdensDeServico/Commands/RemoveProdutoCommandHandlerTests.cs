using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveProduto;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Commands;

public class RemoveProdutoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly RemoveProdutoCommandHandler _handler;

    public RemoveProdutoCommandHandlerTests()
    {
        _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new RemoveProdutoCommandHandler(
            _ordemServicoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoProdutoExiste()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;
        var produto = Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca").Data!;
        ordemDeServico.AdicionarProduto(produto, 1);

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new RemoveProdutoCommand(ordemDeServico.Id, produto.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _ordemServicoRepositoryMock.Verify(r => r.Update(ordemDeServico), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoOrdemDeServicoNaoExiste()
    {
        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico?)null);

        var command = new RemoveProdutoCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarBadRequest_QuandoProdutoNaoExisteNaOrdem()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);

        var command = new RemoveProdutoCommand(ordemDeServico.Id, Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
