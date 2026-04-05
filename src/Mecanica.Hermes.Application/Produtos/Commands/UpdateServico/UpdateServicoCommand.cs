using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.UpdateServico;

public sealed record UpdateServicoCommand(
    Guid Id,
    string Descricao,
    decimal Valor) : IRequest<Result<ServicoDto>>;