using Mecanica.Hermes.Application.Clientes.Ports;
using Mecanica.Hermes.Application.Common.Persistence;
using Mecanica.Hermes.Domain.Clientes;
using Mecanica.Hermes.Infrastructure.Persistence;
using Mecanica.Hermes.Infrastructure.Persistence.Repositories;
using Mecanica.Hermes.Infrastructure.Tests.Persistence.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Mecanica.Hermes.Infrastructure.Tests.Persistence.Repositories;

public class ClienteRepositoryTests
{
    private readonly string _dbName = Guid.NewGuid().ToString();

    private AppDbContext CreateContext() =>
        new(InMemoryDbContextFactory.CreateOptions(_dbName));

    private IClienteRepository CreateRepository(AppDbContext db) =>
        new ClienteRepository(db, new Mock<IUnitOfWork>().Object);

    [Fact]
    public async Task GetByIdAsync_Should_ReturnCliente_When_ClienteExists()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;

        await using (var writeDb = CreateContext())
        {
            var repo = CreateRepository(writeDb);
            await repo.AddAsync(cliente);
            await writeDb.SaveChangesAsync();
        }

        await using var readDb = CreateContext();
        var readRepo = CreateRepository(readDb);
        var result = await readRepo.GetByIdAsync(cliente.Id);

        result.Should().NotBeNull();
        result!.NomeCivil.Valor.Should().Be("João Silva");
        result.Email.Valor.Should().Be("joao@example.com");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_ClienteDoesNotExist()
    {
        await using var db = CreateContext();
        var result = await CreateRepository(db).GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_Should_PersistCliente_And_AssignId()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;

        await using (var writeDb = CreateContext())
        {
            await CreateRepository(writeDb).AddAsync(cliente);
            await writeDb.SaveChangesAsync();
        }

        cliente.Id.Should().NotBeEmpty();

        await using var readDb = CreateContext();
        readDb.Clientes.IgnoreQueryFilters().Should().HaveCount(1);
    }

    [Fact]
    public async Task AddAsync_Should_CollectDomainEvents_From_UnitOfWork()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        var uowMock = new Mock<IUnitOfWork>();

        await using var db = CreateContext();
        var repo = new ClienteRepository(db, uowMock.Object);
        await repo.AddAsync(cliente);

        uowMock.Verify(u => u.CollectEventsFrom(cliente), Times.Once);
    }

    [Fact]
    public async Task Update_Should_PersistChanges_When_ClienteIsModified()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;

        await using (var writeDb = CreateContext())
        {
            await CreateRepository(writeDb).AddAsync(cliente);
            await writeDb.SaveChangesAsync();
        }

        cliente.AtualizarDados("Maria Santos", null, "maria@example.com", "11999887766");

        await using var updateDb = CreateContext();
        var updateRepo = CreateRepository(updateDb);
        updateRepo.Update(cliente);
        await updateDb.SaveChangesAsync();
        updateDb.ChangeTracker.Clear();

        var result = await updateRepo.GetByIdAsync(cliente.Id);

        result.Should().NotBeNull();
        result!.NomeCivil.Valor.Should().Be("Maria Santos");
        result.Email.Valor.Should().Be("maria@example.com");
    }

    [Fact]
    public async Task Update_Should_CollectDomainEvents_From_UnitOfWork()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;

        await using (var writeDb = CreateContext())
        {
            await CreateRepository(writeDb).AddAsync(cliente);
            await writeDb.SaveChangesAsync();
        }

        var uowMock = new Mock<IUnitOfWork>();
        cliente.AtualizarDados("Maria Santos", null, "maria@example.com", "11999887766");

        await using var updateDb = CreateContext();
        var repo = new ClienteRepository(updateDb, uowMock.Object);
        repo.Update(cliente);

        uowMock.Verify(u => u.CollectEventsFrom(cliente), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_IncludeVeiculos_When_ClienteHasVeiculos()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;
        cliente.AdicionarVeiculo("Civic", "Honda", "ABC1234", 2020);

        await using (var writeDb = CreateContext())
        {
            await CreateRepository(writeDb).AddAsync(cliente);
            await writeDb.SaveChangesAsync();
        }

        await using var readDb = CreateContext();
        var result = await CreateRepository(readDb).GetByIdAsync(cliente.Id);

        result.Should().NotBeNull();
        result!.Veiculos.Should().HaveCount(1);
        result.Veiculos.First().Modelo.Should().Be("Civic");
    }

    [Fact]
    public async Task GetByIdAsync_Should_ReturnNull_When_ClienteIsSoftDeleted()
    {
        var cliente = Cliente.Criar("João Silva", null, "12345678909", "joao@example.com", "11987654321").Data!;

        await using (var writeDb = CreateContext())
        {
            await CreateRepository(writeDb).AddAsync(cliente);
            await writeDb.SaveChangesAsync();
        }

        cliente.Excluir();

        await using var updateDb = CreateContext();
        var updateRepo = CreateRepository(updateDb);
        updateRepo.Update(cliente);
        await updateDb.SaveChangesAsync();
        updateDb.ChangeTracker.Clear();

        var result = await updateRepo.GetByIdAsync(cliente.Id);

        result.Should().BeNull();
    }
}
