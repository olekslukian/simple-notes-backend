using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SimpleNotesApp.Infrastructure.Data;


public class DbContext(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public async Task<IEnumerable<T>> QueryAsync<T>(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return await dbConnection.QueryAsync<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<T?> QuerySingleAsync<T>(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return await dbConnection.QuerySingleOrDefaultAsync<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> ExecuteAsync(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return await dbConnection.ExecuteAsync(procedure, parameters, commandType: CommandType.StoredProcedure) > 0;
    }

    public IEnumerable<T> Query<T>(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return dbConnection.Query<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public T? QuerySingle<T>(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return dbConnection.QuerySingleOrDefault<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public bool Execute(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return dbConnection.Execute(procedure, parameters, commandType: CommandType.StoredProcedure) > 0;
    }

}
