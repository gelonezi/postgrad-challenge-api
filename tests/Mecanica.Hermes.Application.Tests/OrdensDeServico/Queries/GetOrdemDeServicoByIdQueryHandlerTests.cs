using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.OrdensDeServico.Queries.GetOrdemDeServicoById;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.OrdensDeServico;

namespace Mecanica.Hermes.Application.Tests.OrdensDeServico.Queries;

public class GetOrdemDeServicoByIdQueryHandlerTests
{
    private readonly Mock<IOrdemServicoRepository> _repositoryMock;
    private readonly GetOrdemDeServicoByIdQueryHandler _handler;

    public GetOrdemDeServicoByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IOrdemServicoRepository>();
        _handler = new GetOrdemDeServicoByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_DeveRetornarSucesso_QuandoOrdemDeServicoExiste()
    {
        var clienteResult = Cliente.Criar("João Silva", null, "11144477735", "joao@email.com", "11999999999");
        var cliente = clienteResult.Data!;
        var veiculoResult = cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);
        var veiculo = veiculoResult.Data!;
        var ordemDeServico = OrdemDeServico.Criar(cliente, veiculo, "Problema", "Observação").Data!;

        _repositoryMock.Setup(r => r.GetByIdAsync(ordemDeServico.Id)).ReturnsAsync((OrdemDeServico?)ordemDeServico);

        var query = new GetOrdemDeServicoByIdQuery(ordemDeServico.Id);

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(ordemDeServico.Id, result.Data.Id);
        _repositoryMock.Verify(r => r.GetByIdAsync(ordemDeServico.Id), Times.Once);
    }

    [Fact]
    public async Task Handle_DeveRetornarNotFound_QuandoOrdemDeServicoNaoExiste()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((OrdemDeServico?)null);

        var query = new GetOrdemDeServicoByIdQuery(Guid.NewGuid());

        var result = await _handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(System.Net.HttpStatusCode.NotFound, result.StatusCode);
    }
}
