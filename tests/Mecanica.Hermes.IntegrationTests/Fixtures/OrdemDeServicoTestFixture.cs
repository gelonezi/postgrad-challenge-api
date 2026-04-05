using System.Text.Json;
using System.Text.Json.Serialization;
using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;
using Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.IntegrationTests.Builders;
using Mecanica.Hermes.IntegrationTests.Infrastructure;
using OrdemDeServicoContracts = Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;

namespace Mecanica.Hermes.IntegrationTests.Fixtures;

public class OrdemDeServicoTestFixture
{
    protected readonly HttpClient Client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public OrdemDeServicoTestFixture(IntegrationTestWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected async Task<ProdutoResponse> CriarProdutoAsync(ProdutoBuilder? builder = null)
    {
        builder ??= new ProdutoBuilder();
        var request = builder.Build();
        
        var response = await Client.PostAsJsonAsync("/api/produtos", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create produto. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new HttpRequestException($"Response was successful but body is empty. Status: {response.StatusCode}, Headers: {response.Headers}");
        }
        
        try
        {
            return await response.Content.ReadFromJsonAsync<ProdutoResponse>(JsonOptions) ?? throw new InvalidOperationException("Deserialization returned null");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"Failed to deserialize response. Content: {content}, Error: {ex.Message}", ex);
        }
    }

    protected async Task<ServicoResponse> CriarServicoAsync(ServicoBuilder? builder = null)
    {
        builder ??= new ServicoBuilder();
        var request = builder.Build();
        
        var response = await Client.PostAsJsonAsync("/api/servicos", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create servico. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<ServicoResponse>(JsonOptions))!;
    }

    protected async Task<ClienteResponse> CriarClienteAsync(ClienteBuilder? builder = null)
    {
        builder ??= new ClienteBuilder();
        var request = builder.Build();
        
        var response = await Client.PostAsJsonAsync("/api/clientes", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create cliente. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<ClienteResponse>(JsonOptions))!;
    }

    protected async Task<VeiculoResponse> CriarVeiculoAsync(Guid clienteId, VeiculoBuilder? builder = null)
    {
        builder ??= new VeiculoBuilder();
        var request = builder.Build();
        
        var response = await Client.PostAsJsonAsync($"/api/clientes/{clienteId}/veiculos", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create veiculo for cliente {clienteId}. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        var content = await response.Content.ReadAsStringAsync();
        var cliente = System.Text.Json.JsonSerializer.Deserialize<ClienteResponse>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        
        if (cliente.Veiculos == null || !cliente.Veiculos.Any())
        {
            throw new InvalidOperationException($"Cliente response does not contain veiculos. ClienteId: {clienteId}, VeiculosCount: {cliente.Veiculos?.Count ?? 0}, Response: {content}");
        }
        
        return cliente.Veiculos.Last();
    }

    protected async Task<OrdemDeServicoContracts.OrdemDeServicoResponse> CriarOrdemDeServicoAsync(
        Guid clienteId,
        Guid veiculoId,
        string? problemaRelatado = null,
        string? observacoes = null)
    {
        var request = new OrdemDeServicoContracts.CreateOrdemDeServicoRequest(
            clienteId,
            veiculoId,
            problemaRelatado ?? "Problema padrão",
            observacoes);
        
        var response = await Client.PostAsJsonAsync("/api/ordens-de-servico", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to create ordem de servico for cliente {clienteId} and veiculo {veiculoId}. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<OrdemDeServicoContracts.OrdemDeServicoResponse>(JsonOptions))!;
    }

    protected async Task<OrdemDeServicoContracts.OrdemDeServicoResponse> AdicionarProdutoAsync(
        Guid ordemId,
        Guid produtoId,
        int quantidade = 1)
    {
        var request = new OrdemDeServicoContracts.AddOrdemServicoProdutoRequest(produtoId, quantidade);
        var response = await Client.PostAsJsonAsync($"/api/ordens-de-servico/{ordemId}/produtos", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to add produto {produtoId} to ordem {ordemId}. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<OrdemDeServicoContracts.OrdemDeServicoResponse>(JsonOptions))!;
    }

    protected async Task<OrdemDeServicoContracts.OrdemDeServicoResponse> AdicionarServicoAsync(
        Guid ordemId,
        Guid servicoId)
    {
        var request = new OrdemDeServicoContracts.AddOrdemServicoServicoRequest(servicoId);
        var response = await Client.PostAsJsonAsync($"/api/ordens-de-servico/{ordemId}/servicos", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to add servico {servicoId} to ordem {ordemId}. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<OrdemDeServicoContracts.OrdemDeServicoResponse>(JsonOptions))!;
    }

    protected async Task<OrdemDeServicoContracts.OrdemDeServicoResponse> AvancarEtapaAsync(Guid ordemId)
    {
        var response = await Client.PatchAsync($"/api/ordens-de-servico/{ordemId}/avancar", null);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to avancar etapa for ordem {ordemId}. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<OrdemDeServicoContracts.OrdemDeServicoResponse>(JsonOptions))!;
    }

    protected async Task<OrdemDeServicoContracts.OrdemDeServicoResponse> CancelarOrdemAsync(Guid ordemId)
    {
        var response = await Client.PatchAsync($"/api/ordens-de-servico/{ordemId}/cancelar", null);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to cancelar ordem {ordemId}. Status: {response.StatusCode}, Error: {errorContent}");
        }
        
        return (await response.Content.ReadFromJsonAsync<OrdemDeServicoContracts.OrdemDeServicoResponse>(JsonOptions))!;
    }

    protected async Task<OrdemDeServicoContracts.OrdemDeServicoResponse> ObterOrdemPorIdAsync(Guid ordemId)
    {
        var response = await Client.GetAsync($"/api/ordens-de-servico/{ordemId}");
        response.EnsureSuccessStatusCode();
        
        return (await response.Content.ReadFromJsonAsync<OrdemDeServicoContracts.OrdemDeServicoResponse>(JsonOptions))!;
    }

    protected async Task<HttpResponseMessage> RemoverProdutoAsync(Guid ordemId, Guid produtoId)
    {
        return await Client.DeleteAsync($"/api/ordens-de-servico/{ordemId}/produtos/{produtoId}");
    }

    protected async Task<HttpResponseMessage> RemoverServicoAsync(Guid ordemId, Guid servicoId)
    {
        return await Client.DeleteAsync($"/api/ordens-de-servico/{ordemId}/servicos/{servicoId}");
    }

    protected async Task<PagedResult<OrdemDeServicoContracts.OrdemDeServicoResponse>> ListarPorStatusAsync(string status, int page = 1, int pageSize = 10)
    {
        var response = await Client.GetAsync($"/api/ordens-de-servico/status/{status}?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<PagedResult<OrdemDeServicoContracts.OrdemDeServicoResponse>>(JsonOptions))!;
    }
}
