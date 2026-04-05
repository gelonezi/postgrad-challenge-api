using Mecanica.Hermes.Application.Clientes.Dtos;
using Mecanica.Hermes.Domain.Common.Results;
using MediatR;

namespace Mecanica.Hermes.Application.Clientes.Queries.GetClienteByIdentificacaoFiscal;

public sealed record GetClienteByIdentificacaoFiscalQuery(string IdentificacaoFiscal) : IRequest<Result<ClienteDto>>;