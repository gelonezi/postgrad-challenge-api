using System.Net;
using FluentAssertions;
using Mecanica.Hermes.Domain.Produtos.Enums;
using Mecanica.Hermes.IntegrationTests.Builders;
using Mecanica.Hermes.IntegrationTests.Fixtures;
using Mecanica.Hermes.IntegrationTests.Infrastructure;

namespace Mecanica.Hermes.IntegrationTests.OrdensDeServico;

[Collection(IntegrationTestCollection.Name)]
public class OrdemDeServicoRemoveTests : OrdemDeServicoTestFixture
{
    public OrdemDeServicoRemoveTests(IntegrationTestWebApplicationFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task RemoverProduto_DeveRetornarSucesso_QuandoProdutoExisteNaOrdem()
    {
        var uid = Math.Abs(Guid.NewGuid().GetHashCode()) % 9000 + 1000;
        var produto = await CriarProdutoAsync(
            new ProdutoBuilder()
                .ComDescricao("Filtro de Ar")
                .ComValor(35.00m)
                .ComQuantidade(10)
                .DoTipo(TipoProduto.Peca));

        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Remove Produto")
                .ComCpf("33839178002")
                .ComEmail("rm.prod@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComPlaca($"RPA{uid:D4}")
                .ComAno(2021));

        var ordem = await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, "Revisão");
        await AdicionarProdutoAsync(ordem.Id, produto.Id);

        var response = await RemoverProdutoAsync(ordem.Id, produto.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoverProduto_DeveRetornarNotFound_QuandoOrdemNaoExiste()
    {
        var response = await RemoverProdutoAsync(Guid.NewGuid(), Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoverProduto_DeveRetornarBadRequest_QuandoProdutoNaoExisteNaOrdem()
    {
        var uid = Math.Abs(Guid.NewGuid().GetHashCode()) % 9000 + 1000;
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("BadReq Produto")
                .ComCpf("64202097068")
                .ComEmail("rm.prod.bad@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComPlaca($"BRP{uid:D4}")
                .ComAno(2020));

        var ordem = await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, "Revisão");

        var response = await RemoverProdutoAsync(ordem.Id, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RemoverServico_DeveRetornarSucesso_QuandoServicoExisteNaOrdem()
    {
        var uid = Math.Abs(Guid.NewGuid().GetHashCode()) % 9000 + 1000;
        var servico = await CriarServicoAsync(
            new ServicoBuilder()
                .ComDescricao("Alinhamento")
                .ComValor(90.00m));

        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("Remove Servico")
                .ComCpf("61061650090")
                .ComEmail("rm.serv@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComPlaca($"RSA{uid:D4}")
                .ComAno(2019));

        var ordem = await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, "Revisão");
        await AdicionarServicoAsync(ordem.Id, servico.Id);

        var response = await RemoverServicoAsync(ordem.Id, servico.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemoverServico_DeveRetornarNotFound_QuandoOrdemNaoExiste()
    {
        var response = await RemoverServicoAsync(Guid.NewGuid(), Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemoverServico_DeveRetornarBadRequest_QuandoServicoNaoExisteNaOrdem()
    {
        var uid = Math.Abs(Guid.NewGuid().GetHashCode()) % 9000 + 1000;
        var cliente = await CriarClienteAsync(
            new ClienteBuilder()
                .ComNome("BadReq Servico")
                .ComCpf("30099050099")
                .ComEmail("rm.serv.bad@email.com"));

        var veiculo = await CriarVeiculoAsync(cliente.Id,
            new VeiculoBuilder()
                .ComPlaca($"BSP{uid:D4}")
                .ComAno(2022));

        var ordem = await CriarOrdemDeServicoAsync(cliente.Id, veiculo.Id, "Revisão");

        var response = await RemoverServicoAsync(ordem.Id, Guid.NewGuid());

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
