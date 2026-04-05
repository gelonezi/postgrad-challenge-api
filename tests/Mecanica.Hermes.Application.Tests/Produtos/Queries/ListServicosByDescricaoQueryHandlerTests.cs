using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Application.Produtos.Queries.ListServicosByDescricao;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos;
using Mecanica.Hermes.Domain.Produtos.ValueObjects;

namespace Mecanica.Hermes.Application.Tests.Produtos.Queries;

public class ListServicosByDescricaoQueryHandlerTests
{
    private readonly Mock<IServicoRepository> _repositoryMock;
    private readonly ListServicoByDescricaoQueryHandler _handler;

    public ListServicosByDescricaoQueryHandlerTests()
    {
        _repositoryMock = new Mock<IServicoRepository>();
        _handler = new ListServicoByDescricaoQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnPaginatedServicos()
    {
        var servicos = new List<Servico>
        {
            Servico.Criar("Troca de Óleo", 80.00m).Data!,
            Servico.Criar("Troca de Filtro", 60.00m).Data!
        };

        var pagedResult = PagedResult<Servico>.Create(servicos, 1, 10, 2);
        var query = new ListServicoByDescricaoQuery("Troca", 1, 10);
        
        _repositoryMock
            .Setup(x => x.ListByDescricaoAsync(It.IsAny<DescricaoProdutoVo>(), It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_NoServicosFound()
    {
        var pagedResult = PagedResult<Servico>.Create(new List<Servico>(), 1, 10, 0);
        var query = new ListServicoByDescricaoQuery("Inexistente", 1, 10);
        
        _repositoryMock
            .Setup(x => x.ListByDescricaoAsync(It.IsAny<DescricaoProdutoVo>(), It.IsAny<PaginationParams>()))
            .ReturnsAsync(pagedResult);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().BeEmpty();
        result.Data.TotalCount.Should().Be(0);
    }
}
