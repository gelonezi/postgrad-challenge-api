using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Application.Produtos.Queries.GetServicoById;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Tests.Produtos.Queries;

public class GetServicoByIdQueryHandlerTests
{
    private readonly Mock<IServicoRepository> _repositoryMock;
    private readonly GetServicoByIdQueryHandler _handler;

    public GetServicoByIdQueryHandlerTests()
    {
        _repositoryMock = new Mock<IServicoRepository>();
        _handler = new GetServicoByIdQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_ServicoDoesNotExist()
    {
        var query = new GetServicoByIdQuery(Guid.NewGuid());
        _repositoryMock.Setup(x => x.GetByIdAsync(query.Id)).ReturnsAsync((Servico?)null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnServico_When_ServicoExists()
    {
        var servico = Servico.Criar("Troca de Óleo", 80.00m).Data!;
        var query = new GetServicoByIdQuery(servico.Id);
        _repositoryMock.Setup(x => x.GetByIdAsync(query.Id)).ReturnsAsync(servico);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(servico.Id);
        result.Data.Descricao.Should().Be("Troca de Óleo");
        result.Data.Valor.Should().Be(80.00m);
    }
}
