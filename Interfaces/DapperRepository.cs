using System.Data;
using Dapper;

namespace WebApplication1.Interfaces
{
    public class DapperRepository : IDapperRepository
    {
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return await DbConnection.EstablishConnection().QueryAsync<T>(query, parameters, commandType: commandType);
        }
        public IEnumerable<T> Query<T>(string query, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            return DbConnection.EstablishConnection().Query<T>(query, parameters, commandType:commandType);
        }
        public T QuerySingleOrDefault<T>(string query, DynamicParameters parameters, CommandType type = CommandType.StoredProcedure)
        {
            var data = DbConnection.EstablishConnection().QuerySingleOrDefault<T>(query, parameters, commandType: type);
            if (data == null)
            {
                throw new Exception("No data found for the given query.");
            }
            return data;
        }
        public void Execute(string query,DynamicParameters parameters,CommandType type = CommandType.StoredProcedure)
        {
            DbConnection.EstablishConnection().Execute(query, parameters, commandType: type);
        }
    }
}
