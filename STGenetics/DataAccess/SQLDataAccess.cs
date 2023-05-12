using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace STGenetics.DataAccess
{
    internal class SQLDataAccess : ISQLDataAccess
    {
        private readonly IConfiguration configuration;
        public SQLDataAccess(IConfiguration configuration) 
        {
            this.configuration = configuration;
        }

        public async Task<IEnumerable<T>> GetData<T, Param>(string spName, Param parameters, string connection = "conn")
        {
            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString(connection));
            return await dbConnection.QueryAsync<T>(spName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task SaveData<Param>(string spName, Param parameters, string connection = "conn")
        { 
            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString(connection));
            await dbConnection.ExecuteAsync(spName, parameters, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<T>> GetDataByQuery<T, Param>(string query, Param parameters, string connection = "conn")
        {
            using IDbConnection dbConnection = new SqlConnection(configuration.GetConnectionString(connection));
            return await dbConnection.QueryAsync<T>(query, parameters);
        }
    }
}
