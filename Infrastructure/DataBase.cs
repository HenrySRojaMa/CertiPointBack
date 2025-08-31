using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using InfrastructureDomain;
using Microsoft.Data.SqlClient;
using System.Data;

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
    }
}
