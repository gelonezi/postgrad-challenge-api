using System.Globalization;
using System.Text;
using Mecanica.Hermes.Application.Common.Constants;
using Mecanica.Hermes.Application.Common.Dtos;
using Mecanica.Hermes.Application.Common.Ports;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;
using Mecanica.Hermes.Domain.Common.Results;
using Mecanica.Hermes.Domain.OrdensDeServico;
using Mecanica.Hermes.Domain.OrdensDeServico.Enums;
using MediatR;

namespace Mecanica.Hermes.Application.OrdensDeServico.Commands.SolicitarAprovacaoOrdemDeServico;

internal class SolicitarAprovacaoOrdemDeServicoCommandHandler(
    IOrdemServicoRepository ordemServicoRepository,
    IEmailSenderService emailSender)
    : IRequestHandler<SolicitarAprovacaoOrdemDeServicoCommand, Result>
{
    public async Task<Result> Handle(SolicitarAprovacaoOrdemDeServicoCommand request,
        CancellationToken cancellationToken)
    {
        var ordemDeServico = await ordemServicoRepository.GetByIdAsync(request.OrdemDeServicoId);
        if (ordemDeServico is null)
            return Result.NotFound();

        if (ordemDeServico.StatusAtual.StatusAtual != OrdemDeServicoStatus.AguardandoAprovacao)
            return Result.BadRequest(
                "Ordem de serviço precisa estar em aguardando aprovação para enviar solicitação ao cliente");

        var email = OrdemDeServicoPendenteAprovacao(ordemDeServico);
        await emailSender.SendAsync(email);

        return Result.Ok();
    }

    private static EmailMessage OrdemDeServicoPendenteAprovacao(OrdemDeServico ordemDeServico)
    {
        return new EmailMessage(ordemDeServico.Cliente.Email,
            "Orçamento disponível para aprovação - Mecânica Hermes",
            MontarCorpoDeEmailAguardandoAprovacao(
                ordemDeServico));
    }

    private static string MontarCorpoDeEmailAguardandoAprovacao(OrdemDeServico ordemDeServico)
    {
        var baseUrl = Environment.GetEnvironmentVariable("APP__BASE_URL");
        var urlAprovar =
            $"{baseUrl}/scalar/#tag/ordens-de-servico/PATCH/api/ordens-de-servico/{ordemDeServico.Id}/aprovar";
        var urlRejeitar =
            $"{baseUrl}/scalar/#tag/ordens-de-servico/PATCH/api/ordens-de-servico/{ordemDeServico.Id}/rejeitar";

        var sb = new StringBuilder();

        sb.Append($"""
                       <p>Olá, <b>{ordemDeServico.Cliente.NomeSocial ?? ordemDeServico.Cliente.NomeCivil}</b>,</p>
                       <p>
                           O orçamento da sua ordem de serviço está disponível e
                           <b>aguarda sua análise</b>.
                       </p>

                       <ul>
                           <li><b>Ordem de serviço</b>: {ordemDeServico.Id}</li>
                           <li><b>Valor total estimado</b>: {ordemDeServico.ValorTotal.ToString("C", CultureInfo.GetCultureInfo(ApplicationConstants.PtBrCulture))}</li>
                       </ul>

                       <p><b>Resumo dos itens do orçamento:</b></p>
                       <ul>
                   """);

        if (ordemDeServico.Produtos.Count != 0)
        {
            foreach (var item in ordemDeServico.Produtos)
            {
                sb.Append($"""
                               <li>
                                   {item.Descricao}
                                   {item.Quantidade} x {item.Valor.ToString("C", CultureInfo.GetCultureInfo(ApplicationConstants.PtBrCulture))}
                                   = <b>{item.ValorTotal.ToString("C", CultureInfo.GetCultureInfo(ApplicationConstants.PtBrCulture))}</b>
                               </li>
                           """);
            }
        }

        if (ordemDeServico.Servicos.Count != 0)
        {
            foreach (var item in ordemDeServico.Servicos)
            {
                sb.Append($"""
                               <li>
                                   {item.Descricao}
                                   = <b>{item.Valor.ToString("C", CultureInfo.GetCultureInfo(ApplicationConstants.PtBrCulture))}</b>
                               </li>
                           """);
            }
        }

        sb.Append($$"""
                        </ul>

                        <p>
                            Para prosseguir, escolha uma das opções abaixo:
                        </p>

                        <div style="margin-top:16px;">
                            <a href="{{urlAprovar}}" target="_blank"
                               style="display:inline-block;padding:12px 20px;background:#28a745;color:#fff;
                                      text-decoration:none;border-radius:4px;font-weight:bold;">
                               Aprovar orçamento
                            </a>

                            &nbsp;&nbsp;

                            <a href="{{urlRejeitar}}" target="_blank"
                               style="display:inline-block;padding:12px 20px;background:#dc3545;color:#fff;
                                      text-decoration:none;border-radius:4px;font-weight:bold;">
                               Rejeitar orçamento
                            </a>
                        </div>

                        <hr style="margin-top:32px;border:none;border-top:1px solid #e5e5e5;" />
                    <p style="font-size: 13px; color: #555;">
                        <b>Mecânica Hermes</b><br/>
                        Especialistas em manutenção automotiva<br/><br/>

                        Este é um e-mail automático. Caso tenha dúvidas, entre em contato conosco
                        pelos nossos canais oficiais.<br/><br/>

                        - {{DateTime.Now.Year}} Mecânica Hermes. Todos os direitos reservados.
                    </p>
                    """);

        return sb.ToString();
    }
}
