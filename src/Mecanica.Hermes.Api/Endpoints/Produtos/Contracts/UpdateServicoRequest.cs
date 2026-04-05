using System.ComponentModel.DataAnnotations;

namespace Mecanica.Hermes.Api.Endpoints.Produtos.Contracts;

public sealed record UpdateServicoRequest(
    [Required(AllowEmptyStrings = false)] string Descricao,
    [Required] decimal Valor);