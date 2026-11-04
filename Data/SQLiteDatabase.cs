using SQLite;
using CRUD_Empleados.Models;

namespace CRUD_Empleados.Data;

/// Maneja la conexión e inicialización de la base de datos SQLite
/// y las operaciones sobre la tabla de empleados.
public class SQLiteDatabase
{
    private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;

    private readonly string _dbPath;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private SQLiteAsyncConnection? _database;

    public SQLiteDatabase(string dbPath)
    {
        _dbPath = dbPath;
    }

    // Abre la conexión y crea la tabla la primera vez que se necesita,
    // sin bloquear el hilo de la interfaz (sin .Wait()).
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_database is not null)
            return _database;

        await _initLock.WaitAsync();
        try
        {
            if (_database is null)
            {
                var connection = new SQLiteAsyncConnection(_dbPath, Flags);
                await connection.CreateTableAsync<Empleados>();
                _database = connection;
            }

            return _database;
        }
        finally
        {
            _initLock.Release();
        }
    }

    // Create: guarda un empleado. Devuelve el número de filas insertadas.
    // Tras insertar, empleado.Id contiene el ID generado.
    public async Task<int> GuardarEmpleadoAsync(Empleados empleado)
    {
        var db = await GetConnectionAsync();
        return await db.InsertAsync(empleado);
    }

    // Read: todos los empleados, del más reciente al más antiguo.
    public async Task<List<Empleados>> ObtenerEmpleadosAsync()
    {
        var db = await GetConnectionAsync();
        return await db.Table<Empleados>()
                       .OrderByDescending(e => e.Id)
                       .ToListAsync();
    }

    // Read by Id
    public async Task<Empleados?> ObtenerEmpleadoPorIdAsync(int id)
    {
        var db = await GetConnectionAsync();
        return await db.FindAsync<Empleados>(id);
    }

    // Update
    public async Task<int> ActualizarEmpleadoAsync(Empleados empleado)
    {
        var db = await GetConnectionAsync();
        return await db.UpdateAsync(empleado);
    }

    // Delete
    public async Task<int> EliminarEmpleadoAsync(Empleados empleado)
    {
        var db = await GetConnectionAsync();
        return await db.DeleteAsync(empleado);
    }
}