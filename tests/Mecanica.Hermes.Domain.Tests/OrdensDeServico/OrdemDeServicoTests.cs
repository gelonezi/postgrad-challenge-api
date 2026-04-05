using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Domain.Tests.OrdensDeServico;

public class OrdemDeServicoTests
{
    private readonly Cliente _cliente;
    private readonly Veiculo _veiculo;
    private readonly Produto _produto;
    private readonly Servico _servico;

    public OrdemDeServicoTests()
    {
        var clienteResult = Cliente.Criar(
            "João Silva",
            null,
            "11144477735",
            "joao@email.com",
            "11999999999");
        
        if (clienteResult.IsFailure)
            throw new InvalidOperationException($"Failed to create cliente: {string.Join(", ", clienteResult.Errors)}");
        
        _cliente = clienteResult.Data!;

        var veiculoResult = _cliente.AdicionarVeiculo(
            "Civic",
            "Honda",
            "ABC1234",
            2020);
        
        if (veiculoResult.IsFailure)
            throw new InvalidOperationException($"Failed to add veiculo: {string.Join(", ", veiculoResult.Errors)}");
        
        _veiculo = veiculoResult.Data!;

        var produtoResult = Produto.Criar(
            "Óleo de Motor",
            50.00m,
            10,
            "Peca");
        
        if (produtoResult.IsFailure)
            throw new InvalidOperationException($"Failed to create produto: {string.Join(", ", produtoResult.Errors)}");
        
        _produto = produtoResult.Data!;

        var servicoResult = Servico.Criar(
            "Troca de Óleo",
            100.00m);
        
        if (servicoResult.IsFailure)
            throw new InvalidOperationException($"Failed to create servico: {string.Join(", ", servicoResult.Errors)}");
        
        _servico = servicoResult.Data!;
    }

    [Fact]
    public void Criar_DeveRetornarSucesso_QuandoDadosValidos()
    {
        var result = OrdemDeServico.Criar(
            _cliente,
            _veiculo,
            "Motor fazendo barulho",
            "Cliente relatou barulho ao acelerar");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(_cliente, result.Data.Cliente);
        Assert.Equal(_veiculo, result.Data.Veiculo);
        Assert.Equal("Motor fazendo barulho", result.Data.ProblemaRelatado);
        Assert.Equal("Cliente relatou barulho ao acelerar", result.Data.Observacoes);
        Assert.Equal("Recebida", result.Data.StatusAtual.StatusAtual.ToString());
    }

    [Fact]
    public void AdicionarProduto_DeveRetornarSucesso_QuandoStatusPermiteEdicao()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;

        var result = ordemDeServico.AdicionarProduto(_produto, 2);

        Assert.True(result.IsSuccess);
        Assert.Single(ordemDeServico.Produtos);
        Assert.Equal(_produto.Id, ordemDeServico.Produtos.First().ProdutoId);
    }

    [Fact]
    public void AdicionarProduto_DeveRetornarErro_QuandoProdutoJaAdicionado()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        ordemDeServico.AdicionarProduto(_produto, 1);

        var result = ordemDeServico.AdicionarProduto(_produto, 1);

        Assert.True(result.IsFailure);
        Assert.Contains("já foi adicionado", result.Errors.First());
    }

    [Fact]
    public void RemoverProduto_DeveRetornarSucesso_QuandoProdutoExiste()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        ordemDeServico.AdicionarProduto(_produto, 1);

        var result = ordemDeServico.RemoverProduto(_produto.Id);

        Assert.True(result.IsSuccess);
        Assert.True(ordemDeServico.Produtos.Single().IsDeleted);
    }

    [Fact]
    public void RemoverProduto_DeveRetornarErro_QuandoProdutoNaoExiste()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;

        var result = ordemDeServico.RemoverProduto(Guid.NewGuid());

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void AdicionarServico_DeveRetornarSucesso_QuandoStatusPermiteEdicao()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;

        var result = ordemDeServico.AdicionarServico(_servico);

        Assert.True(result.IsSuccess);
        Assert.Single(ordemDeServico.Servicos);
        Assert.Equal(_servico.Id, ordemDeServico.Servicos.First().ServicoId);
    }

    [Fact]
    public void AdicionarServico_DeveRetornarErro_QuandoServicoJaAdicionado()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        ordemDeServico.AdicionarServico(_servico);

        var result = ordemDeServico.AdicionarServico(_servico);

        Assert.True(result.IsFailure);
        Assert.Contains("já foi adicionado", result.Errors.First());
    }

    [Fact]
    public void RemoverServico_DeveRetornarSucesso_QuandoServicoExiste()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        ordemDeServico.AdicionarServico(_servico);

        var result = ordemDeServico.RemoverServico(_servico.Id);

        Assert.True(result.IsSuccess);
        Assert.True(ordemDeServico.Servicos.Single().IsDeleted);
    }

    [Fact]
    public void RemoverServico_DeveRetornarErro_QuandoServicoNaoExiste()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;

        var result = ordemDeServico.RemoverServico(Guid.NewGuid());

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void ValorTotal_DeveCalcularCorretamente_ComProdutosEServicos()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        ordemDeServico.AdicionarProduto(_produto, 3); // 50.00 * 3 = 150.00
        ordemDeServico.AdicionarServico(_servico); // 100.00

        var valorTotal = ordemDeServico.ValorTotal;

        Assert.Equal(250.00m, valorTotal);
    }

    [Fact]
    public void ValorTotal_DeveExcluirProdutosRemovidosDoCalculo()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        ordemDeServico.AdicionarProduto(_produto, 2); // 50.00 * 2 = 100.00
        ordemDeServico.AdicionarServico(_servico); // 100.00
        ordemDeServico.RemoverProduto(_produto.Id);

        var valorTotal = ordemDeServico.ValorTotal;

        Assert.Equal(100.00m, valorTotal);
    }

    [Fact]
    public void AvancarEtapa_DeveAdicionarHistorico()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;
        var statusInicial = ordemDeServico.StatusAtual.StatusAtual;

        var result = ordemDeServico.AvancarEtapa();

        Assert.True(result.IsSuccess);
        Assert.NotEqual(statusInicial, ordemDeServico.StatusAtual.StatusAtual);
        Assert.Single(ordemDeServico.HistoricosStatus);
    }

    [Fact]
    public void Cancelar_DeveAdicionarHistorico()
    {
        var ordemDeServico = OrdemDeServico.Criar(_cliente, _veiculo, null, null).Data!;

        var result = ordemDeServico.Cancelar();

        Assert.True(result.IsSuccess);
        Assert.Equal("Cancelada", ordemDeServico.StatusAtual.StatusAtual.ToString());
        Assert.Single(ordemDeServico.HistoricosStatus);
    }
}
