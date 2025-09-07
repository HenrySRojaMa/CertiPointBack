using EntitiesDomain.Responses;
using Microsoft.Data.SqlClient;

namespace InfrastructureDomain
{
    public interface IDataBase : ITransaction
    {
        Task<Response<SqlConnection>> GetConnection();
        Task<Response<(SqlConnection, SqlTransaction)>> GetTransaction();
        Task<Response<int>> Execute(string spName, object parameters = null, int? timeOut = null);
        Task<Response<List<T>>> Query<T>(string spName, object parameters = null, int? timeOut = null);
        Task<Response<(List<A>, List<B>)>> QueryMultiple<A, B>(string spName, object parameters = null, int? timeOut = null);
        Task<Response<(List<A>, List<B>, List<C>, List<D>?, List<E>?)>> QueryMultiple<A, B, C, D, E>(string spName, object parameters = null, int? timeOut = null);
        Task<Response<(List<A>, List<B>, List<C>, List<D>, List<E>, List<F>, List<G>?, List<H>?, List<I>?, List<J>?)>> QueryMultiple<A, B, C, D, E, F, G, H, I, J>(string spName, object parameters = null, int? timeOut = null);
    }
}
