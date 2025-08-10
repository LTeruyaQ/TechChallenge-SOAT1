using Infraestrutura.Dados;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace MecanicaOSTests.Fixtures;

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    // Esta classe não precisa de implementação
    // É usada apenas para definir a coleção de testes
}

public class DatabaseFixture : IDisposable
{
    public MecanicaContexto Context { get; private set; }
    private readonly SqliteConnection _connection;

    public DatabaseFixture()
    {
        // Configuração do banco de dados em memória
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<MecanicaContexto>()
            .UseSqlite(_connection)
            .Options;

        Context = new MecanicaContexto(options);

        // Garante que o banco de dados seja criado
        Context.Database.EnsureCreated();

        // Inicializa os dados de teste
        ResetDatabase();
    }

    public void ResetDatabase()
    {
        // Limpa os dados existentes
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();

        // Aqui você pode adicionar dados de teste iniciais, se necessário
        // Por exemplo, criar clientes, veículos, serviços, etc.

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
