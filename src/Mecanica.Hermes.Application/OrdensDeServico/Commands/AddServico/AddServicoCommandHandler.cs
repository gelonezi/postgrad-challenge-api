using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AddServico;

internal class AddServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IServicoRepository servicoRepository,
    IUnitOfWork uow)
    : IRequestHandler<AddServicoCommand, Result<OrdemDeServicoDto>>
{
    public async Task<Result<OrdemDeServicoDto>> Handle(AddServicoCommand request, CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var servico = await servicoRepository.GetByIdAsync(request.ServicoId);
        if (servico is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var result = ordemDeServico.AdicionarServico(servico);
        if (result.IsFailure)
            return Result<OrdemDeServicoDto>.BadRequest(result.Errors);

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        return Result<OrdemDeServicoDto>.Ok(ordemDeServico.ToDto());
    }
}
