using Mecanica.Hermes.Domain.Clientes.ValueObjects;

namespace Mecanica.Hermes.Infrastructure.Persistence.Configurations.Helpers;

internal static class IdentificacaoFiscalVoHelper
{
    public static IdentificacaoFiscalVo RecriarIdentificacaoFiscal(string valor)
    {
        var resultado = IdentificacaoFiscalVo.Criar(valor);
        if (resultado.IsSuccess && resultado.Data is not null)
            return resultado.Data;

        throw new InvalidOperationException("Não foi possível reconstruir a identificação fiscal armazenada.");
    }
}