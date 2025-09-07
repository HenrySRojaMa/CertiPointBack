using EntitiesDomain.Queries;
using EntitiesDomain.Responses;

namespace ApplicationDomain
{
    public interface ISystemBusiness
    {
        Task<Response<List<CatalogItem>>> ListCatalogsService(CatalogListQuery query);
    }
}
