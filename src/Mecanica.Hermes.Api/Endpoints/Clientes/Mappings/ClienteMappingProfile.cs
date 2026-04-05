using AutoMapper;
using Mecanica.Hermes.Application.Clientes.Commands.AddCliente;
using Mecanica.Hermes.Application.Clientes.Dtos;

namespace Mecanica.Hermes.Api.Endpoints.Clientes.Contracts;

public class ClienteMappingProfile : Profile
{
    public ClienteMappingProfile()
    {
        CreateMap<ClienteDto, ClienteResponse>();
        CreateMap<VeiculoDto, VeiculoResponse>();
        CreateMap<AddClienteRequest, AddClienteCommand>();
    }
}