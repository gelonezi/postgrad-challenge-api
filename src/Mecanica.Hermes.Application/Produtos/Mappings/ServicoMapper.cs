using Mecanica.Hermes.Application.Produtos.Dtos;
using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Application.Produtos.Mappings;

internal static class ServicoMapper
{
    public static ServicoDto ToDto(this Servico servico)
    {
        return new ServicoDto(
            servico.Id,
            servico.Descricao.Valor,
            servico.Valor.Valor);
    }
}
