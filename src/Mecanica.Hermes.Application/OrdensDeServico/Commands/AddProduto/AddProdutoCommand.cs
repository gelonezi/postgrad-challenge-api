using Mecanica.Hermes.Application.OrdensDeServico.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.AddProduto;

public sealed record AddProdutoCommand(
    Guid OrdemDeServicoId,
    Guid ProdutoId,
    int Quantidade) : IRequest<Result<OrdemDeServicoDto>>;
