using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace SimpleNotesApp.Data;


public class DbContext(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public IEnumerable<T> LoadData<T>(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return dbConnection.Query<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public T LoadDataSingle<T>(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return dbConnection.QuerySingle<T>(procedure, parameters, commandType: CommandType.StoredProcedure);
    }

    public bool ExecuteSql(string procedure, object? parameters = null)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        using IDbConnection dbConnection = new SqlConnection(connectionString);
        return dbConnection.Execute(procedure, parameters, commandType: CommandType.StoredProcedure) > 0;
    }

}