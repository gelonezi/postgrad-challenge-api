using System.Diagnostics.Metrics;
using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.OrdensDeServico.Ports;

namespace Mecanica.Hermes.Infrastructure.Observability;

public sealed class BusinessMetrics : IClienteMetrics, IOrdemDeServicoMetrics, IDisposable
{
    public const string MeterName = "MecanicaHermes.Business";

    private readonly Meter _meter;
    private readonly Counter<long> _clientesCriados;
    private readonly Counter<long> _ordemCriadas;
    private readonly Counter<long> _etapaAvancada;
    private readonly Counter<long> _erros;
    private readonly Histogram<double> _duracaoEtapa;

    public BusinessMetrics()
    {
        _meter = new Meter(MeterName);
        _clientesCriados = _meter.CreateCounter<long>(
            "cliente.criados",
            description: "Número de clientes criados com sucesso");
        _ordemCriadas = _meter.CreateCounter<long>(
            "ordemservico.criadas",
            description: "Número de ordens de serviço criadas");
        _etapaAvancada = _meter.CreateCounter<long>(
            "ordemservico.etapa_avancada",
            description: "Número de avanços de etapa em ordens de serviço");
        _erros = _meter.CreateCounter<long>(
            "ordemservico.erros",
            description: "Número de erros em operações de ordem de serviço");
        _duracaoEtapa = _meter.CreateHistogram<double>(
            "ordemservico.duracao_etapa_ms",
            unit: "ms",
            description: "Tempo gasto em cada etapa da ordem de serviço");
    }

    public void ClienteCriado() => _clientesCriados.Add(1);

    public void OrdemCriada() => _ordemCriadas.Add(1);

    public void EtapaAvancada(string statusAnterior, string statusNovo) =>
        _etapaAvancada.Add(1,
            new KeyValuePair<string, object?>("status_anterior", statusAnterior),
            new KeyValuePair<string, object?>("status_novo", statusNovo));

    public void ErroRegistrado(string operacao, string tipoErro) =>
        _erros.Add(1,
            new KeyValuePair<string, object?>("operacao", operacao),
            new KeyValuePair<string, object?>("tipo_erro", tipoErro));

    public void DuracaoEtapaRegistrada(string status, double duracaoMs) =>
        _duracaoEtapa.Record(duracaoMs,
            new KeyValuePair<string, object?>("status", status));

    public void Dispose() => _meter.Dispose();
}
