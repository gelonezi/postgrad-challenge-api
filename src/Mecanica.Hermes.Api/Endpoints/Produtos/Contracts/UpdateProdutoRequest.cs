using System.ComponentModel.DataAnnotations;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;

public sealed record UpdateProdutoRequest(
    [Required(AllowEmptyStrings = false)] string Descricao,
    [Required] int Quantidade,
    [Required] decimal Valor);