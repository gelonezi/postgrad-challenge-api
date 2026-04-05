using Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;

namespace Mecanica.Hermes.IntegrationTests.Builders;

public class ServicoBuilder
{
    private string _descricao = "Serviço Teste";
    private decimal _valor = 100.00m;

    public ServicoBuilder ComDescricao(string descricao)
    {
        _descricao = descricao;
        return this;
    }

    public ServicoBuilder ComValor(decimal valor)
    {
        _valor = valor;
        return this;
    }

    public AddServicoRequest Build()
    {
        return new AddServicoRequest(_descricao, _valor);
    }
}
