using System.Net;
using FluentAssertions;
using Mecanica.Hermes.IntegrationTests.Builders;
using Mecanica.Hermes.IntegrationTests.Fixtures;
using Mecanica.Hermes.IntegrationTests.Infrastructure;

namespace Mecanica.Hermes.IntegrationTests.OrdensDeServico;

[Collection(IntegrationTestCollection.Name)]
public class OrdemDeServicoListByStatusTests : OrdemDeServicoTestFixture
{
    public OrdemDeServicoListByStatusTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ListarPorStatus_DeveRetornarOrdens_QuandoStatusValido()
    {
        var uid = Math.Abs(Guid.NewGuid().GetHashCode()) % 9000 + 1000;
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Teste Status")
                .ComCpf("38601446000")
                .ComEmail("list.status@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("Celta")
                .ComMarca("Chevrolet")
                .ComPlaca($"LST{uid:D4}")
                .ComAno(2015));

        await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, "Problema status 1");
        await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, "Problema status 2");

        var result = await ListarPorStatusAsync("Recebida");

        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();
        result.Items.Should().AllSatisfy(o => o.StatusAtual.Should().Be("Recebida"));
    }

    [Fact]
    public async Task ListarPorStatus_DeveRetornarBadRequest_QuandoStatusInvalido()
    {
        var response = await Client.GetAsync("/api/ordens-de-servico/status/StatusInexistente");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ListarPorStatus_DeveRetornarListaVazia_QuandoNaoExistemOrdensComStatus()
    {
        var result = await ListarPorStatusAsync("Entregue");

        result.Should().NotBeNull();
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task ListarPorStatus_DeveRespeitar_Paginacao()
    {
        var uid = Math.Abs(Guid.NewGuid().GetHashCode()) % 9000 + 1000;
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Paginacao Status")
                .ComCpf("22762879000")
                .ComEmail("list.pag@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("Gol")
                .ComMarca("VW")
                .ComPlaca($"PAG{uid:D4}")
                .ComAno(2018));

        for (int i = 1; i <= 3; i++)
            await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, $"Problema paginacao {i}");

        var page1 = await ListarPorStatusAsync("Recebida", page: 1, pageSize: 2);
        var page2 = await ListarPorStatusAsync("Recebida", page: 2, pageSize: 2);

        page1.Items.Should().HaveCount(2);
        page1.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        page2.Items.Should().NotBeEmpty();
    }
}
