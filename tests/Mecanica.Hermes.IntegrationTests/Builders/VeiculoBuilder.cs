using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;

namespace Mecanica.Hermes.IntegrationTests.Builders;

public class VeiculoBuilder
{
    private string _modelo = "Modelo Teste";
    private string _marca = "Marca Teste";
    private string _placa = "ABC1234";
    private int _ano = 2020;

    public VeiculoBuilder ComModelo(string modelo)
    {
        _modelo = modelo;
        return this;
    }

    public VeiculoBuilder ComMarca(string marca)
    {
        _marca = marca;
        return this;
    }

    public VeiculoBuilder ComPlaca(string placa)
    {
        _placa = placa;
        return this;
    }

    public VeiculoBuilder ComAno(int ano)
    {
        _ano = ano;
        return this;
    }

    public AddVeiculoRequest Build()
    {
        return new AddVeiculoRequest(_modelo, _marca, _placa, _ano);
    }
}
