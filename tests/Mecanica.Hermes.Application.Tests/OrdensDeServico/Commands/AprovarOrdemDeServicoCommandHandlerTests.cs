using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.AprovarOrdemDeServico;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.OrdensDeServico;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Commands;

public class AprovarOrdemDeServicoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrdemDeServicoMetrics> _metricsMock;
    private readonly AprovarOrdemDeServicoCommandHandler _handler;

    public AprovarOrdemDeServicoCommandHandlerTests()
    {
        _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _metricsMock = new Mock<IOrdemDeServicoMetrics>();
        _handler = new AprovarOrdemDeServicoCommandHandler(
            _ordemServicoRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _metricsMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarBadRequest_QuandoOrdemNaoEstaEmAguardandoAprovacao()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new AprovarOrdemDeServicoCommand(ordemDeServico.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Null(result.Data);
        _ordemServicoRepositoryMock.Verify(r => r.Update(ordemDeServico), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoOrdemNaoExiste()
    {
        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico?)null);

        var command = new AprovarOrdemDeServicoCommand(Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarOk_QuandoOrdemEstaEmAguardandoAprovacao()
    {
        var cliente = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999").Data!;
        var veiculo = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020).Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, null, null).Data!;
        ordemDeServico.AvancarEtapa(); // Recebida → EmDiagnostico
        ordemDeServico.AvancarEtapa(); // EmDiagnostico → AguardandoAprovacao

        _ordemServicoRepositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);

        var command = new AprovarOrdemDeServicoCommand(ordemDeServico.Id);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.Update(It.IsAny<OrdemDeServico>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
