using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;

namespace Mecanica.Hermes.IntegrationTests.Builders;

public class ClienteBuilder
{
    private string _nomeCivil = "Cliente Teste";
    private string? _nomeSocial = null;
    private string _cpf = "48783319093"; // Valid CPF: 487.833.190-93
    private string _email = "cliente@teste.com";
    private string _telefone = "11999999999";

    public ClienteBuilder ComNome(string nomeCivil)
    {
        _nomeCivil = nomeCivil;
        return this;
    }

    public ClienteBuilder ComNomeSocial(string nomeSocial)
    {
        _nomeSocial = nomeSocial;
        return this;
    }

    public ClienteBuilder ComCpf(string cpf)
    {
        _cpf = cpf;
        return this;
    }

    public ClienteBuilder ComEmail(string email)
    {
        _email = email;
        return this;
    }

    public ClienteBuilder ComTelefone(string telefone)
    {
        _telefone = telefone;
        return this;
    }

    public AddClienteRequest Build()
    {
        return new AddClienteRequest(_nomeCivil, _nomeSocial, _cpf, _email, _telefone);
    }
}
