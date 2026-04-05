using Mecanica.Hermes.Application.Common.Dtos;

namespace Mecanica.Hermes.Application.Common.Ports;

public interface IEmailSenderService
{
    Task SendAsync(EmailMessage message);
}