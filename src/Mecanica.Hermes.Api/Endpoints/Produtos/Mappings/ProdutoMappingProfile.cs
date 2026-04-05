using AutoMapper;
using Mecanica.Hermes.Application.Produtos.Commands.AddProduto;
using Mecanica.Hermes.Application.Produtos.Dtos;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;

public class ProdutoMappingProfile : Profile
{
    public ProdutoMappingProfile()
    {
        CreateMap<AddProdutoRequest, AddProdutoCommand>();
        CreateMap<ProdutoDto, ProdutoResponse>();
    }
}
