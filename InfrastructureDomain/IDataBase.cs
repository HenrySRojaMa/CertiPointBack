using EntitiesDomain.Responses;
using Microsoft.Data.SqlClient;

namespace InfrastructureDomain
{
    public interface IDataBase : ITransaction
    {
        Task<Response<SqlConnection>> GetConnection();
        Task<Response<(SqlConnection, SqlTransaction)>> GetTransaction();
    }
}
