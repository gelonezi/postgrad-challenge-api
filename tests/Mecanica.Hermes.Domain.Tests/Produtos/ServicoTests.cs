using Mecanica.Hermes.Domain.Produtos;

namespace Mecanica.Hermes.Domain.Tests.Produtos;

public class ServicoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveRetornarServicoComEvento()
    {
        var result = Servico.Criar("Troca de Óleo", 150.00m);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Descricao.Valor.Should().Be("Troca de Óleo");
        result.Data.Valor.Valor.Should().Be(150.00m);
        result.Data.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void AtualizarDados_DeveModificarServicoEAdicionarEvento()
    {
        var servico = CriarServicoPadrao();

        var result = servico.AtualizarDados("Troca de Óleo Completa", 200.00m);

        result.IsSuccess.Should().BeTrue();
        servico.Descricao.Valor.Should().Be("Troca de Óleo Completa");
        servico.Valor.Valor.Should().Be(200.00m);
        servico.DomainEvents.Should().HaveCount(2);
    }

    private static Servico CriarServicoPadrao()
    {
        return Servico.Criar("Troca de Óleo", 150.00m).Data!;
    }
}
