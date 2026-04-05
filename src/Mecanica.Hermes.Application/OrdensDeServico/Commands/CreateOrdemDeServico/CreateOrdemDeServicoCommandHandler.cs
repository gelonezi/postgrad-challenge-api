using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.OrdensDeServico;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.CreateOrdemDeServico;

internal class CreateOrdemDeServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IClienteRepository clienteRepository,
    IUnitOfWork uow,
    IOrdemDeServicoMetrics metrics)
    : IRequestHandler<CreateOrdemDeServicoCommand, Result<OrdemDeServicoDto>>
{
    public async Task<Result<OrdemDeServicoDto>> Handle(CreateOrdemDeServicoCommand request, CancellationToken cancellationToken)
    {
        var cliente = await clienteRepository.GetByIdAsync(request.ClienteId);
        if (cliente is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var veiculo = cliente.Veiculos.FirstOrDefault(v => v.Id == request.VeiculoId);
        if (veiculo is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var ordemDeServico = OrdemDeServico.Criar(
            cliente,
            veiculo,
            request.ProblemaRelatado,
            request.Observacoes);

        if (ordemDeServico.IsFailure)
        {
            metrics.ErroRegistrado("criar", "validacao");
            return Result<OrdemDeServicoDto>.BadRequest(ordemDeServico.Errors);
        }

        await ordemServicoRepository.AddAsync(ordemDeServico.Data!);
        await uow.CommitAsync(cancellationToken);

        metrics.OrdemCriada();

        return Result<OrdemDeServicoDto>.Ok(ordemDeServico.Data!.ToDto());
    }
}
