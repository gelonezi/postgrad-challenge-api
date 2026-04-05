using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.UpdateProduto;

public sealed record UpdateProdutoCommand(
    Guid Id,
    string Descricao,
    decimal Valor) : IRequest<Result<ProdutoDto>>;
