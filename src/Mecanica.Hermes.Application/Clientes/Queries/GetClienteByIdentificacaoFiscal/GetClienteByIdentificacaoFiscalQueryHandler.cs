using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Application.Clientes.Mappings;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Domain.Clientes.ValueObjects;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.GetClienteByIdentificacaoFiscal;

internal class GetClienteByIdentificacaoFiscalQueryHandler(IClienteRepository repository)
    : IRequestHandler<GetClienteByIdentificacaoFiscalQuery, Result<ClienteDto>>
{
    public async Task<Result<ClienteDto>> Handle(GetClienteByIdentificacaoFiscalQuery request,
        CancellationToken cancellationToken)
    {
        var identificacaoFiscal = IdentificacaoFiscalVo.Criar(request.IdentificacaoFiscal);
        if (identificacaoFiscal.IsFailure)
            return Result<ClienteDto>.BadRequest(identificacaoFiscal.Errors);

        var cliente = await repository.GetByIdentificacaoFiscalAsync(identificacaoFiscal.Data!);
        return cliente == null
            ? Result<ClienteDto>.NotFound()
            : Result<ClienteDto>.Ok(cliente.ToDto());
    }
}