using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.AddProduto;

public sealed record AddProdutoCommand(
    string Descricao,
    decimal Valor,
    int Quantidade,
    string Tipo) : IRequest<Result<ProdutoDto>>;
