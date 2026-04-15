using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MinhasFinancas.Infrastructure;
using MinhasFinancas.Infrastructure.Data;
using MinhasFinancas.Infrastructure.Queries;
using MinhasFinancas.Application.Services;

namespace Backend.IntegrationTests.Support;

public abstract class SqliteIntegrationTestBase : IAsyncLifetime
{
    private SqliteConnection? _connection;
    protected MinhasFinancasDbContext DbContext { get; private set; } = null!;
    protected UnitOfWork UnitOfWork { get; private set; } = null!;
    protected PessoaService PessoaService { get; private set; } = null!;
    protected CategoriaService CategoriaService { get; private set; } = null!;
    protected TransacaoService TransacaoService { get; private set; } = null!;
    protected TotalService TotalService { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync();

        var options = new DbContextOptionsBuilder<MinhasFinancasDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new MinhasFinancasDbContext(options);
        await DbContext.Database.EnsureCreatedAsync();

        UnitOfWork = new UnitOfWork(DbContext);
        PessoaService = new PessoaService(UnitOfWork);
        CategoriaService = new CategoriaService(UnitOfWork);
        TransacaoService = new TransacaoService(UnitOfWork);

        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var totaisQuery = new TotaisQuery(DbContext, memoryCache);
        TotalService = new TotalService(totaisQuery);
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }
    }
}
