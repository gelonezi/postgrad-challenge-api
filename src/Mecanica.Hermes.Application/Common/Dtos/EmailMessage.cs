using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Application.Common.Dtos;

public record EmailMessage(EmailVo To, string Subject, string Body);