namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;

public sealed record AddOrdemServicoProdutoRequest(Guid ProdutoId, int Quantidade);
