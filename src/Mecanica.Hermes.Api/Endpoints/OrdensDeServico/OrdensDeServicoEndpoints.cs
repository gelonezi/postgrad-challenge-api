using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Routes;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico;

public static class OrdensDeServicoEndpoints
{
    public static void MapOrdensDeServicoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ordens-de-servico")
            .WithTags("Ordens de Servico");

        group.MapCreateOrdemDeServico();
        group.MapGetOrdemDeServicoById();
        group.MapListAllOrdensDeServico();
        group.MapListOrdemDeServicoByStatus();
        group.MapAddProduto();
        group.MapAddServico();
        group.MapRemoveProduto();
        group.MapRemoveServico();
        group.MapAvancarEtapa();
        group.MapCancelarOrdemDeServico();
        group.MapSolicitarAprovacaoOrdemDeServico();
        group.MapAprovarOrdemDeServico();
        group.MapRejeitarOrdemDeServico();
    }
}
