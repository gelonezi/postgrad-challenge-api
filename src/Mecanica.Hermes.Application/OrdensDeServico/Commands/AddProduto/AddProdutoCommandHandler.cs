using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Application.OrdensDeServico.Mappings;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AddProduto;

internal class AddProdutoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IProdutoRepository produtoRepository,
    IUnitOfWork uow)
    : IRequestHandler<AddProdutoCommand, Result<OrdemDeServicoDto>>
{
    public async Task<Result<OrdemDeServicoDto>> Handle(AddProdutoCommand request, CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var produto = await produtoRepository.GetByIdAsync(request.ProdutoId);
        if (produto is null)
            return Result<OrdemDeServicoDto>.NotFound();

        var result = ordemDeServico.AdicionarProduto(produto, request.Quantidade);
        if (result.IsFailure)
            return Result<OrdemDeServicoDto>.BadRequest(result.Errors);

        ordemServicoRepository.Update(ordemDeServico);
        await uow.CommitAsync(cancellationToken);

        return Result<OrdemDeServicoDto>.Ok(ordemDeServico.ToDto());
    }
}
