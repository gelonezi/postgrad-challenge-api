using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Domain.Common.Abstractions;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.OrdensDeServico.Abstractions;
using Mecanica.Hermes.Domain.OrdensDeServico.Events;
using Mecanica.Hermes.Domain.OrdensDeServico.Status;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Domain.OrdensDeServico;

public class OrdemDeServico : AggregateRoot
{
    private OrdemDeServico(Cliente cliente,
        Veiculo veiculo,
        string? problemaRelatado,
        string? observacoes,
        OrdemDeServicoStatusBase statusAtual,
        IList<OrdemDeServicoHistoricoStatus>? historicosStatus = null,
        ICollection<OrdemDeServicoProduto>? produtos = null,
        ICollection<OrdemDeServicoServico>? servicos = null)
    {
        Cliente = cliente;
        Veiculo = veiculo;
        ProblemaRelatado = problemaRelatado;
        Observacoes = observacoes;
        StatusAtual = statusAtual;
        HistoricosStatus = historicosStatus ?? [];
        Produtos = produtos ?? [];
        Servicos = servicos ?? [];
    }

    public IList<OrdemDeServicoHistoricoStatus> HistoricosStatus { get; set; }
    public OrdemDeServicoStatusBase StatusAtual { get; private set; }
    public Cliente Cliente { get; private set; }
    public Veiculo Veiculo { get; private set; }
    public string? ProblemaRelatado { get; private set; }
    public string? Observacoes { get; private set; }
    public ICollection<OrdemDeServicoProduto> Produtos { get; private set; }
    public ICollection<OrdemDeServicoServico> Servicos { get; private set; }
    public decimal ValorTotal => Produtos.Where(p => !p.IsDeleted).Sum(p => p.Valor * p.Quantidade) + Servicos.Where(s => !s.IsDeleted).Sum(s => s.Valor);

    public static Result<OrdemDeServico> Criar(
        Cliente cliente,
        Veiculo veiculo,
        string? problemaRelatado,
        string? observacoes)
    {
        var statusInicial = new OrdemDeServicoRecebida();
        var ordemServico = new OrdemDeServico(cliente, veiculo, problemaRelatado, observacoes, statusInicial);

        ordemServico.AddDomainEvent(new OrdemDeServicoCriadoEvent(
            ordemServico.Id,
            cliente.Id,
            veiculo.Id,
            problemaRelatado,
            statusInicial.StatusAtual));

        return Result<OrdemDeServico>.Ok(ordemServico);
    }

    internal static OrdemDeServico Restaurar(
        Guid id,
        Cliente cliente,
        Veiculo veiculo,
        string? problemaRelatado,
        string? observacoes,
        OrdemDeServicoStatusBase statusAtual,
        IList<OrdemDeServicoHistoricoStatus>? historicoStatus,
        ICollection<OrdemDeServicoProduto>? produtos,
        ICollection<OrdemDeServicoServico>? servicos,
        DateTime createdAt,
        DateTime? updatedAt,
        bool isDeleted)
    {
        var ordemServico = new OrdemDeServico(
            cliente,
            veiculo,
            problemaRelatado,
            observacoes,
            statusAtual,
            historicoStatus,
            produtos,
            servicos);

        ordemServico.RestaurarBase(id, createdAt, updatedAt, isDeleted);
        return ordemServico;
    }

    public Result<OrdemDeServicoProduto> AdicionarProduto(Produto produto, int quantidade)
    {
        if (!StatusAtual.PermiteEditarProdutos)
            return Result<OrdemDeServicoProduto>.BadRequest(
                "O status atual da ordem de serviço não permite edição de produtos.");

        if (quantidade <= 0)
            return Result<OrdemDeServicoProduto>.BadRequest(
                "A quantidade deve ser maior que zero.");

        if (Produtos.Any(p => p.ProdutoId == produto.Id))
            return Result<OrdemDeServicoProduto>.BadRequest(
                "O produto já foi adicionado à ordem de serviço.");

        var ordemDeServicoProduto = OrdemDeServicoProduto.Criar(produto, quantidade);
        Produtos.Add(ordemDeServicoProduto);

        return Result<OrdemDeServicoProduto>.Ok(ordemDeServicoProduto);
    }

    public Result<OrdemDeServicoProduto> RemoverProduto(Guid produtoId)
    {
        if (!StatusAtual.PermiteEditarProdutos)
            return Result<OrdemDeServicoProduto>.BadRequest(
                "O status atual da ordem de serviço não permite edição de produtos.");

        var produto = Produtos.FirstOrDefault(p => p.ProdutoId == produtoId);
        if (produto == null)
            return Result<OrdemDeServicoProduto>.BadRequest("Produto não encontrado.");

        produto.MarkAsDeleted();

        return Result<OrdemDeServicoProduto>.Ok();
    }

    public Result<OrdemDeServicoServico> AdicionarServico(Servico servico)
    {
        if (!StatusAtual.PermiteEditarProdutos)
            return Result<OrdemDeServicoServico>.BadRequest(
                "O status atual da ordem de serviço não permite edição de serviços.");

        if (Servicos.Any(s => s.ServicoId == servico.Id))
            return Result<OrdemDeServicoServico>.BadRequest(
                "O serviço já foi adicionado à ordem de serviço.");

        var ordemDeServicoServico = OrdemDeServicoServico.Criar(servico);
        Servicos.Add(ordemDeServicoServico);

        return Result<OrdemDeServicoServico>.Ok(ordemDeServicoServico);
    }

    public Result<OrdemDeServicoServico> RemoverServico(Guid servicoId)
    {
        if (!StatusAtual.PermiteEditarProdutos)
            return Result<OrdemDeServicoServico>.BadRequest(
                "O status atual da ordem de serviço não permite edição de serviços.");

        var servico = Servicos.FirstOrDefault(s => s.ServicoId == servicoId);

        if (servico == null)
            return Result<OrdemDeServicoServico>.BadRequest("Serviço não encontrado.");

        servico.MarkAsDeleted();

        return Result<OrdemDeServicoServico>.Ok();
    }

    public Result AvancarEtapa()
    {
        var statusAnterior = StatusAtual;
        var novoStatus = StatusAtual.AvancarEtapa(AddDomainEvent);
        if (novoStatus.IsFailure)
            return Result.BadRequest(novoStatus.Errors);

        StatusAtual = novoStatus.Data!;
        HistoricosStatus.Add(new OrdemDeServicoHistoricoStatus(statusAnterior));
        AddDomainEvent(new OrdemDeServicoEtapaAvancadaEvent(Id, statusAnterior.StatusAtual, novoStatus.Data!.StatusAtual));
        return Result.Ok();
    }

    public Result Cancelar()
    {
        var statusAnterior = StatusAtual;
        var novoStatus = StatusAtual.Cancelar(AddDomainEvent);
        if (novoStatus.IsFailure)
            return Result.BadRequest(novoStatus.Errors);

        StatusAtual = novoStatus.Data!;
        HistoricosStatus.Add(new OrdemDeServicoHistoricoStatus(statusAnterior));
        AddDomainEvent(new OrdemDeServicoCanceladaEvent(Id, statusAnterior.StatusAtual));
        return Result.Ok();
    }
}
