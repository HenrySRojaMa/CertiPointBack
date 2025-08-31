using EntitiesDomain.Responses;

namespace InfrastructureDomain
{
    public interface ITransaction
    {
        Task<Response<bool>> Begin();
        Task<Response<bool>> Commit();
        Task<Response<bool>> Rollback();
    }
}
