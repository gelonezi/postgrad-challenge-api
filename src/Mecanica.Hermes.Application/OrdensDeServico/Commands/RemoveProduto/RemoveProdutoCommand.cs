using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.RemoveProduto;

public sealed record RemoveProdutoCommand(
    Guid OrdemDeServicoId,
    Guid ProdutoId) : IRequest<Result>;
