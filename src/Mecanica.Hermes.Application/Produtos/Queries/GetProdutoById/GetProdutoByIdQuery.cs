using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.GetProdutoById;

public sealed record GetProdutoByIdQuery(Guid Id) : IRequest<Result<ProdutoDto>>;
