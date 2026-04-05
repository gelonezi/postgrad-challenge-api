using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.AddProduto;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Commands;

public class AddProdutoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IProdutoRepository> _produtoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddProdutoCommandHandler _handler;

    public AddProdutoCommandHandlerTests()
    {
        _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
        _produtoRepositoryMock = new Mock<IProdutoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AddProdutoCommandHandler(
            _ordemServicoRepositoryMock.Object,
            _produtoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoDadosValidos()
    {
        var clienteResult = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999");
        var cliente = clienteResult.Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculo = veiculoResult.Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;

        var produtoResult = Produto.Criar("Óleo de Motor", 50.00m, 10, "Peca");
        var produto = produtoResult.Data!;

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);
        _produtoRepositoryMock.Setup(r => r.GetByIdAsync(produto.Id)).ReturnsAsync(produto);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AddProdutoCommand(ordemDeServico.Id, produto.Id, 2);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        _ordemServicoRepositoryMock.Verify(r => r.Update(ordemDeServico), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoOrdemDeServicoNaoExiste()
    {
        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico?)null);

        var command = new AddProdutoCommand(Guid.NewGuid(), Guid.NewGuid(), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoProdutoNaoExiste()
    {
        var clienteResult = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999");
        var cliente = clienteResult.Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculo = veiculoResult.Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);
        _produtoRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Produto?)null);

        var command = new AddProdutoCommand(ordemDeServico.Id, Guid.NewGuid(), 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
    }
}
