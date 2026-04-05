using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Mappings;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Pagination;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.Produtos;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.AddProduto;

internal class AddProdutoCommandHandler(IProdutoRepository repository, IUnitOfWork uow)
    : IRequestHandler<AddProdutoCommand, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(AddProdutoCommand request, CancellationToken cancellationToken)
    {
        var produto = Produto.Criar(
            request.Descricao,
            request.Valor,
            request.Quantidade,
            request.Tipo);

        if (produto.IsFailure)
            return Result<ProdutoDto>.BadRequest(produto.Errors);

        var checkParams = new PaginationParams { Page = 1, PageSize = 1 };
        var produtosExistentes = await repository.ListByDescricaoAsync(produto.Data!.Descricao, checkParams);
        if (produtosExistentes.TotalCount > 0)
            return Result<ProdutoDto>.Conflict("Já existe um produto com esta descrição.");

        await repository.AddAsync(produto.Data!);
        await uow.CommitAsync(cancellationToken);

        return Result<ProdutoDto>.Ok(produto.Data!.ToDto());
    }
}
