using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.DeleteProduto;

public sealed record DeleteProdutoCommand(Guid Id) : IRequest<Result>;
