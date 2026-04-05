using Mecanica.Hermes.Domain.Clientes.ValueObjects;
using Mecanica.Hermes.Domain.Common.Abstractions;

namespace Mecanica.Hermes.Domain.Clientes.Events;

public class VeiculoAdicionadoEvent(Guid id, PlacaVo placa) : IDomainEvent
{
    public Guid Id { get; private set; } = id;
    public PlacaVo Placa { get; private set; } = placa;
}