using Dapper;
using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using InfrastructureDomain;
using Microsoft.Data.SqlClient;
using System.Data;
using static Dapper.SqlMapper;

namespace Infrastructure
{
    public class DataBase : IDataBase, IDisposable
    {
        private readonly string currentClass = typeof(DataBase).FullName;
        private readonly string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        private SqlConnection connection = null;
        private SqlTransaction transaction = null;

        public async Task<Response<SqlConnection>> GetConnection()
        {
            Response<SqlConnection> response = new();
            try
            {
                if (connection == null)
                {
                    connection = new SqlConnection(connectionString);
                    await connection.OpenAsync();
                    if (connection.State == ConnectionState.Open)
                    {
                        response.Code = System.Net.HttpStatusCode.OK;
                        response.Message = "Ok";
                        response.Data = connection;
                    }
                    else
                    {
                        response.Code = System.Net.HttpStatusCode.Unauthorized;
                        response.Message = "There is no connection to the database. Please contact Support.";
                    }
                }
                else
                {
                    response.Code = System.Net.HttpStatusCode.OK;
                    response.Data = connection;
                }
            }
            catch (Exception ex)
            {
                response.Code = System.Net.HttpStatusCode.ServiceUnavailable;
                response.Message = ex.Message;
                response.AddError(ex, currentClass, nameof(GetConnection), false);
            }
            return response;
        }
        public async Task<Response<(SqlConnection, SqlTransaction)>> GetTransaction()
        {
            Response<(SqlConnection, SqlTransaction)> response = new();
            try
            {
                Response<SqlConnection> openedConnection = await GetConnection();
                if (response.IsOk(openedConnection))
                {
                    response.Data = (openedConnection.Data, transaction);
                }
            }
            catch (Exception ex)
            {
                response.Code = System.Net.HttpStatusCode.ServiceUnavailable;
                response.Message = ex.Message;
                response.AddError(ex, currentClass, nameof(GetTransaction), false);
            }
            return response;
        }
        public async Task<Response<bool>> Begin()
        {
            Response<bool> response = new();
            try
            {
                Response<SqlConnection> openedConnection = await GetConnection();
                if (response.IsOk(openedConnection))
                {
                    transaction = openedConnection.Data.BeginTransaction();
                    response.Data = true;
                }
            }
            catch (Exception ex)
            {
                response.Code = System.Net.HttpStatusCode.ServiceUnavailable;
                response.Message = ex.Message;
                response.AddError(ex, currentClass, nameof(Begin), false);
            }
            return response;
        }
        public async Task<Response<bool>> Commit()
        {
            Response<bool> response = new();
            try
            {
                await transaction.CommitAsync();
                response.Code = System.Net.HttpStatusCode.OK;
                response.Message = "Ok";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Code = System.Net.HttpStatusCode.ServiceUnavailable;
                response.Message = ex.Message;
                response.AddError(ex, currentClass, nameof(Commit), false);
            }
            return response;
        }
        public async Task<Response<bool>> Rollback()
        {
            Response<bool> response = new();
            try
            {
                await transaction.RollbackAsync();
                response.Code = System.Net.HttpStatusCode.OK;
                response.Message = "Ok";
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.Code = System.Net.HttpStatusCode.ServiceUnavailable;
                response.Message = ex.Message;
                response.AddError(ex, currentClass, nameof(Rollback), false);
            }
            return response;
        }
        public void Dispose()
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        public async Task<Response<int>> Execute(string spName, object parameters = null, int? timeOut = null)
        {
            Response<int> response = new();
            try
            {
                Response<(SqlConnection, SqlTransaction)> getTransaction = await GetTransaction();
                if (response.IsOk(getTransaction))
                {
                    response.Message = "Procedure completed successfully";
                    response.Data = await connection.ExecuteAsync(spName, parameters, transaction, timeOut, CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(Execute) + "." + spName);
            }
            return response;
        }
        public async Task<Response<List<T>>> Query<T>(string spName, object parameters = null, int? timeOut = null)
        {
            Response<List<T>> response = new();
            try
            {
                Response<(SqlConnection, SqlTransaction)> getTransaction = await GetTransaction();
                if (response.IsOk(getTransaction))
                {
                    response.Message = "Procedure completed successfully";
                    response.Data = (await connection.QueryAsync<T>(spName, parameters, transaction, timeOut, CommandType.StoredProcedure)).ToList();
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(Query) + "." + spName);
            }
            return response;
        }
        public async Task<Response<(List<A>, List<B>)>> QueryMultiple<A, B>(string spName, object parameters = null, int? timeOut = null)
        {
            Response<(List<A>, List<B>)> response = new();

            try
            {
                Response<(SqlConnection, SqlTransaction)> getTransaction = await GetTransaction();
                if (response.IsOk(getTransaction))
                {
                    response.Message = "Procedure completed successfully";
                    GridReader grid = await connection.QueryMultipleAsync(spName, parameters, transaction, timeOut, CommandType.StoredProcedure);
                    List<A> listA = grid.Read<A>().ToList();
                    List<B> listB = grid.Read<B>().ToList();
                    response.Data = (listA, listB);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(QueryMultiple) + "." + spName);
            }
            return response;
        }
        public async Task<Response<(List<A>, List<B>, List<C>, List<D>?, List<E>?)>> QueryMultiple<A, B, C, D, E>(string spName, object parameters = null, int? timeOut = null)
        {
            Response<(List<A>, List<B>, List<C>, List<D>?, List<E>?)> response = new();

            try
            {
                Response<(SqlConnection, SqlTransaction)> getTransaction = await GetTransaction();
                if (response.IsOk(getTransaction))
                {
                    response.Message = "Procedure completed successfully";
                    GridReader grid = await connection.QueryMultipleAsync(spName, parameters, transaction, timeOut, CommandType.StoredProcedure);
                    List<A> listA = grid.Read<A>().ToList();
                    List<B> listB = grid.Read<B>().ToList();
                    List<C> listC = grid.Read<C>().ToList();
                    List<D>? listD = typeof(D) == typeof(object) ? null : grid.Read<D>().ToList();
                    List<E>? listE = typeof(E) == typeof(object) ? null : grid.Read<E>().ToList();
                    response.Data = (listA, listB, listC, listD, listE);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(QueryMultiple) + "." + spName);
            }
            return response;
        }
        public async Task<Response<(List<A>, List<B>, List<C>, List<D>, List<E>, List<F>, List<G>?, List<H>?, List<I>?, List<J>?)>> QueryMultiple<A, B, C, D, E, F, G, H, I, J>(string spName, object parameters = null, int? timeOut = null)
        {
            Response<(List<A>, List<B>, List<C>, List<D>, List<E>, List<F>, List<G>?, List<H>?, List<I>?, List<J>?)> response = new();

            try
            {
                Response<(SqlConnection, SqlTransaction)> getTransaction = await GetTransaction();
                if (response.IsOk(getTransaction))
                {
                    response.Message = "Procedure completed successfully";
                    GridReader grid = await connection.QueryMultipleAsync(spName, parameters, transaction, timeOut, CommandType.StoredProcedure);
                    List<A> listA = grid.Read<A>().ToList();
                    List<B> listB = grid.Read<B>().ToList();
                    List<C> listC = grid.Read<C>().ToList();
                    List<D> listD = grid.Read<D>().ToList();
                    List<E> listE = grid.Read<E>().ToList();
                    List<F> listF = grid.Read<F>().ToList();
                    List<G>? listG = typeof(G) == typeof(object) ? null : grid.Read<G>().ToList();
                    List<H>? listH = typeof(H) == typeof(object) ? null : grid.Read<H>().ToList();
                    List<I>? listI = typeof(I) == typeof(object) ? null : grid.Read<I>().ToList();
                    List<J>? listJ = typeof(J) == typeof(object) ? null : grid.Read<J>().ToList();
                    response.Data = (listA, listB, listC, listD, listE, listF, listG, listH, listI, listJ);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(QueryMultiple) + "." + spName);
            }
            return response;
        }
    }
}
