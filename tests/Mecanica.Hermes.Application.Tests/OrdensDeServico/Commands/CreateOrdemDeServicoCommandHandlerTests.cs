using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.CreateOrdemDeServico;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Clientes;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Commands;

public class CreateOrdemDeServicoCommandHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOrdemDeServicoMetrics> _metricsMock;
    private readonly CreateOrdemDeServicoCommandHandler _handler;

    public CreateOrdemDeServicoCommandHandlerTests()
    {
        _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
        _clienteRepositoryMock = new Mock<IClienteRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _metricsMock = new Mock<IOrdemDeServicoMetrics>();
        _handler = new CreateOrdemDeServicoCommandHandler(
            _ordemServicoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _metricsMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoDadosValidos()
    {
        var clienteResult = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999");
        var cliente = clienteResult.Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculo = veiculoResult.Data!;

        _clienteRepositoryMock.Setup(r => r.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);
        _unitOfWorkMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var command = new CreateOrdemDeServicoCommand(
            cliente.Id,
            veiculo.Id,
            "Motor fazendo barulho",
            "Cliente relatou barulho");

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        _ordemServicoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.OrdensDeServico.OrdemDeServico>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoClienteNaoExiste()
    {
        _clienteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Cliente?)null);

        var command = new CreateOrdemDeServicoCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.OrdensDeServico.OrdemDeServico>()), Times.Never);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoVeiculoNaoExiste()
    {
        var clienteResult = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999");
        var cliente = clienteResult.Data!;

        _clienteRepositoryMock.Setup(r => r.GetByIdAsync(cliente.Id)).ReturnsAsync(cliente);

        var command = new CreateOrdemDeServicoCommand(
            cliente.Id,
            Guid.NewGuid(),
            null,
            null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
        _ordemServicoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.OrdensDeServico.OrdemDeServico>()), Times.Never);
    }
}
