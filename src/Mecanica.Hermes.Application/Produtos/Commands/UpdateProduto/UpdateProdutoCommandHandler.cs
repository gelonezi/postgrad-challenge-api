using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Mappings;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.UpdateProduto;

internal class UpdateProdutoCommandHandler(IProdutoRepository repository, IUnitOfWork uow)
    : IRequestHandler<UpdateProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await repository.GetByIdAsync(request.Id);
        if (produto is null)
            return Result<ProdutoDto>.NotFound();

        var result = produto.AtualizarDados(request.Descricao, request.Valor);
        if (result.IsFailure)
            return Result<ProdutoDto>.BadRequest(result.Errors);

        repository.Update(produto);
        await uow.CommitAsync(cancellationToken);

        return Result<ProdutoDto>.Ok(produto.ToDto());
    }
}
