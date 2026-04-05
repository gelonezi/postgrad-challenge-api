using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.DeleteProduto;

internal class DeleteProdutoCommandHandler(IProdutoRepository repository, IUnitOfWork uow)
    : IRequestHandler<DeleteProdutoCommand, Result>
{
    public async Task<Result> Handle(DeleteProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = await repository.GetByIdAsync(request.Id);
        if (produto is null)
            return Result.NotFound();

        var result = produto.Excluir();
        if (result.IsFailure)
            return Result.BadRequest(result.Errors);

        repository.Update(produto);
        await uow.CommitAsync(cancellationToken);

        return Result.Ok();
    }
}
