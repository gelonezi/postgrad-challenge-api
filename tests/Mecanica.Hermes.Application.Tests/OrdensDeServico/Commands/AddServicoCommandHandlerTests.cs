using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.AddServico;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Commands;

public class AddServicoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IServicoRepository> _servicoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddServicoCommandHandler _handler;

    public AddServicoCommandHandlerTests()
    {
        _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
        _servicoRepositoryMock = new Mock<IServicoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AddServicoCommandHandler(
            _ordemServicoRepositoryMock.Object,
            _servicoRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoDadosValidos()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;
        var servico = Servico.Criar("Troca de Óleo", 100.00m).Data!;

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);
        _servicoRepositoryMock.Setup(r => r.GetByIdAsync(servico.Id)).ReturnsAsync(servico);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AddServicoCommand(ordemDeServico.Id, servico.Id);

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

        var command = new AddServicoCommand(Guid.NewGuid(), Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoServicoNaoExiste()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);
        _servicoRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Servico?)null);

        var command = new AddServicoCommand(ordemDeServico.Id, Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
    }
}
