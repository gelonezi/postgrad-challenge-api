using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.DeleteServico;

public sealed record DeleteServicoCommand(Guid Id) : IRequest<Result>;
