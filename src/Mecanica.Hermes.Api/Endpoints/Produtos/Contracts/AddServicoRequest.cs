using System.ComponentModel.DataAnnotations;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;

public sealed record AddServicoRequest(
    [Required(AllowEmptyStrings = false)] string Descricao,
    [Required] decimal Valor);