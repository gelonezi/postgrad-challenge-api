using AutoMapper;
using Mecanica.Hermes.Application.Produtos.Commands.AddServico;
using Mecanica.Hermes.Application.Produtos.Dtos;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;

public class ServicoMappingProfile : Profile
{
    public ServicoMappingProfile()
    {
        CreateMap<AddServicoRequest, AddServicoCommand>();
        CreateMap<ServicoDto, ServicoResponse>();
    }
}
