using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Queries.GetServicoById;

public sealed record GetServicoByIdQuery(Guid Id) : IRequest<Result<ServicoDto>>;