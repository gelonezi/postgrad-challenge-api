using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Produtos.Commands.AddServico;

public sealed record AddServicoCommand(
    string Descricao,
    decimal Valor) : IRequest<Result<ServicoDto>>;