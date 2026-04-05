using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Application.Produtos.Mappings;
using Mecanica.Hermes.Application.Produtos.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.GetProdutoById;

internal class GetProdutoByIdQueryHandler(IProdutoRepository repository)
    : IRequestHandler<GetProdutoByIdQuery, Result<ProdutoDto>>
{
    public async Task<Result<ProdutoDto>> Handle(GetProdutoByIdQuery request, CancellationToken cancellationToken)
    {
        var produto = await repository.GetByIdAsync(request.Id);

        return produto == null
            ? Result<ProdutoDto>.NotFound()
            : Result<ProdutoDto>.Ok(produto.ToDto());
    }
}
