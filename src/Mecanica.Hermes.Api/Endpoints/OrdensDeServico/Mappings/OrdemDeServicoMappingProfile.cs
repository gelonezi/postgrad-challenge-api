using AutoMapper;
using Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;
using Mecanica.Hermes.Api.Endpoints.OrdensDeServico.Contracts;
using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Commands.CreateOrdemDeServico;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Pagination;

namespace Mecanica.Hermes.Api.Endpoints.OrdensDeServico;

public class OrdemDeServicoMappingProfile : Profile
{
    public OrdemDeServicoMappingProfile()
    {
        CreateMap<CreateOrdemDeServicoRequest, CreateOrdemDeServicoCommand>();

        CreateMap<OrdemDeServicoDto, OrdemDeServicoResponse>();
        CreateMap<OrdemDeServicoProdutoDto, OrdemDeServicoProdutoResponse>();
        CreateMap<OrdemDeServicoServicoDto, OrdemDeServicoServicoResponse>();
        CreateMap<OrdemDeServicoHistoricoStatusDto, OrdemDeServicoHistoricoStatusResponse>();

        CreateMap<ClienteDto, ClienteResponse>();
        CreateMap<VeiculoDto, VeiculoResponse>();

        CreateMap<PagedResult<OrdemDeServicoDto>, PagedResult<OrdemDeServicoResponse>>()
            .ConvertUsing((src, dest, context) => PagedResult<OrdemDeServicoResponse>.Create(
                context.Mapper.Map<List<OrdemDeServicoResponse>>(src.Items),
                src.Page,
                src.PageSize,
                src.TotalCount));
    }
}
