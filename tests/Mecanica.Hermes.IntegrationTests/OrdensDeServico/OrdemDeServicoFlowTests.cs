using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Produtos.Enums;
using Mecanica.Hermes.IntegrationTests.Builders;
using Mecanica.Hermes.IntegrationTests.Fixtures;
using Mecanica.Hermes.IntegrationTests.Infrastructure;
using OrdemDeServicoContracts = Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;

namespace Mecanica.Hermes.IntegrationTests.OrdensDeServico;

[Collection(IntegrationTestCollection.Name)]
public class OrdemDeServicoFlowTests : OrdemDeServicoTestFixture
{
    public OrdemDeServicoFlowTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Scenario1_OrdemDeServico_DeveSerRejeitada()
    {
        // Arrange
        var produto = await CriarProdutoAsync(
            new ProdutoBuilder()
                .ComDescricao("Filtro de Óleo")
                .ComValor(45.00m)
                .ComQuantidade(20)
                .DoTipo(TipoProduto.Peca));

        var servico = await CriarServicoAsync(
            new ServicoBuilder()
                .ComDescricao("Troca de Filtro")
                .ComValor(80.00m));

        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Maria Santos")
                .ComCpf("51125363061")
                .ComEmail("maria@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("Gol")
                .ComMarca("Volkswagen")
                .ComPlaca("ABC1234")
                .ComAno(2018));

        // Act
        var ordem = await CriarOrdemDeServicoAsync(
            cliente.Id,
            veiculo.Id,
            "Motor falhando",
            "Cliente relatou perda de potência");

        ordem.StatusAtual.Should().Be("Recebida");

        var ordemEmDiagnostico = await AvancarEtapaAsync(ordem.Id); // Recebida → EmDiagnostico
        ordemEmDiagnostico.StatusAtual.Should().Be("EmDiagnostico");

        await AdicionarProdutoAsync(ordem.Id, produto.Id);
        await AdicionarServicoAsync(ordem.Id, servico.Id);

        var ordemAguardando = await AvancarEtapaAsync(ordem.Id); // EmDiagnostico → AguardandoAprovacao
        ordemAguardando.StatusAtual.Should().Be("AguardandoAprovacao");

        var ordemRejeitada = await CancelarOrdemAsync(ordem.Id);

        // Assert
        ordemRejeitada.StatusAtual.Should().Be("Rejeitada");
        ordemRejeitada.Produtos.Should().HaveCount(1);
        ordemRejeitada.Servicos.Should().HaveCount(1);
        ordemRejeitada.ValorTotal.Should().Be(125.00m);
    }

    [Fact]
    public async Task Scenario2_OrdemDeServico_DeveSerEntregue()
    {
        // Arrange
        var produto = await CriarProdutoAsync(
            new ProdutoBuilder()
                .ComDescricao("Pastilha de Freio")
                .ComValor(120.00m)
                .ComQuantidade(15)
                .DoTipo(TipoProduto.Peca));

        var servico = await CriarServicoAsync(
            new ServicoBuilder()
                .ComDescricao("Troca de Pastilhas")
                .ComValor(150.00m));

        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("João Silva")
                .ComCpf("87571593000")
                .ComEmail("joao@email.com")
                .ComTelefone("11999887766"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("Civic")
                .ComMarca("Honda")
                .ComPlaca("DEF5678")
                .ComAno(2020));

        // Act
        var ordem = await CriarOrdemDeServicoAsync(
            cliente.Id,
            veiculo.Id,
            "Freios rangendo",
            "Necessário verificar sistema de freios");

        ordem.StatusAtual.Should().Be("Recebida");

        var ordemEmDiagnostico = await AvancarEtapaAsync(ordem.Id); // Recebida → EmDiagnostico
        ordemEmDiagnostico.StatusAtual.Should().Be("EmDiagnostico");

        await AdicionarProdutoAsync(ordem.Id, produto.Id);
        await AdicionarServicoAsync(ordem.Id, servico.Id);

        // Flow: EmDiagnostico -> AguardandoAprovacao -> EmExecucao -> Finalizada -> Entregue
        var ordem1 = await AvancarEtapaAsync(ordem.Id); // EmDiagnostico → AguardandoAprovacao
        ordem1.StatusAtual.Should().Be("AguardandoAprovacao");

        var ordem2 = await AvancarEtapaAsync(ordem.Id);
        ordem2.StatusAtual.Should().Be("EmExecucao");

        var ordem3 = await AvancarEtapaAsync(ordem.Id);
        ordem3.StatusAtual.Should().Be("Finalizada");

        var ordemFinal = await AvancarEtapaAsync(ordem.Id);

        // Assert
        ordemFinal.StatusAtual.Should().Be("Entregue");
        ordemFinal.Produtos.Should().HaveCount(1);
        ordemFinal.Servicos.Should().HaveCount(1);
        ordemFinal.ValorTotal.Should().Be(270.00m);
    }

    [Fact]
    public async Task Scenario3_OrdemDeServico_DeveSerCancelada()
    {
        // Arrange
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Pedro Costa")
                .ComCpf("90831150033")
                .ComEmail("pedro@email.com")
                .ComTelefone("11988776655"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("Corolla")
                .ComMarca("Toyota")
                .ComPlaca("GHI9012")
                .ComAno(2019));

        // Act
        var ordem = await CriarOrdemDeServicoAsync(
            cliente.Id,
            veiculo.Id,
            "Revisão geral");

        ordem.StatusAtual.Should().Be("Recebida");

        var ordemCancelada = await CancelarOrdemAsync(ordem.Id);

        // Assert
        ordemCancelada.StatusAtual.Should().Be("Cancelada");
        ordemCancelada.Produtos.Should().BeEmpty();
        ordemCancelada.Servicos.Should().BeEmpty();
        ordemCancelada.ValorTotal.Should().Be(0);
    }

    [Fact]
    public async Task GetOrdemDeServicoById_DeveRetornarOrdemCriada()
    {
        // Arrange
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Test User")
                .ComCpf("72933793075")
                .ComEmail("test@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("Test Car")
                .ComMarca("Test Brand")
                .ComPlaca("JKL3456")
                .ComAno(2021));

        var ordem = await CriarOrdemDeServicoAsync(
            cliente.Id,
            veiculo.Id,
            "Test problem",
            "Test observation");

        // Act
        var retrievedOrdem = await ObterOrdemPorIdAsync(ordem.Id);

        // Assert
        retrievedOrdem.Should().NotBeNull();
        retrievedOrdem.Id.Should().Be(ordem.Id);
        retrievedOrdem.ProblemaRelatado.Should().Be("Test problem");
    }

    [Fact]
    public async Task ListAllOrdensDeServico_DeveRetornarListaPaginada()
    {
        // Arrange
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("List Test User")
                .ComCpf("67209540083")
                .ComEmail("listtest@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComModelo("List Car")
                .ComMarca("List Brand")
                .ComPlaca("MNO7890")
                .ComAno(2022));

        // Create 3 orders
        for (int i = 1; i <= 3; i++)
        {
            await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, $"Problem {i}");
        }

        // Act
        var listResponse = await Client.GetAsync("/api/ordens-de-servico?page=1&pageSize=10");

        // Assert
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
        var pagedResult = await listResponse.Content.ReadFromJsonAsync<PagedResult<OrdemDeServicoContracts.OrdemDeServicoResponse>>(jsonOptions);
        pagedResult.Should().NotBeNull();
        pagedResult!.Items.Should().HaveCountGreaterThanOrEqualTo(3);
        pagedResult.TotalCount.Should().BeGreaterThanOrEqualTo(3);
    }
}
