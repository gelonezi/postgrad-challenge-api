using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Mappings;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.GetServicoById;

internal class GetServicoByIdQueryHandler : IRequestHandler<GetServicoByIdQuery, Result<ServicoDto>>
{
    private readonly IServicoRepository _repository;

    public GetServicoByIdQueryHandler(IServicoRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ServicoDto>> Handle(GetServicoByIdQuery request, CancellationToken cancellationToken)
    {
        var servico = await _repository.GetByIdAsync(request.Id);

        return servico == null
            ? Result<ServicoDto>.NotFound()
            : Result<ServicoDto>.Ok(servico.ToDto());
    }
}