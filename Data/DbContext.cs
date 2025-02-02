using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace SimpleNotesApp.Data;

// TODO(olekslukian): Refactor to work with stored procedures, wrap in repository

public class DbContext(IConfiguration config)
{
    private readonly IConfiguration _config = config;

    public IEnumerable<T> LoadData<T>(string sql)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        IDbConnection dbConnection = new SqlConnection(connectionString);

        return dbConnection.Query<T>(sql);
    }

    public T LoadDataSingle<T>(string sql)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        IDbConnection dbConnection = new SqlConnection(connectionString);

        return dbConnection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");

        IDbConnection dbConnection = new SqlConnection(connectionString);

        return dbConnection.Execute(sql) > 0;
    }

    public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> sqlParameters)
    {
        string? connectionString = _config.GetConnectionString("DefaultConnection");
        SqlConnection dbConnection = new(connectionString);
        SqlCommand commandWithParams = new(sql);

        foreach (SqlParameter parameter in sqlParameters)
        {
            commandWithParams.Parameters.Add(parameter);
        }

        dbConnection.Open();
        commandWithParams.Connection = dbConnection;
        int rowsAffected = commandWithParams.ExecuteNonQuery();
        dbConnection.Close();

        return rowsAffected > 0;
    }
}