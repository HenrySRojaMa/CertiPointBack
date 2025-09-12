using EntitiesDomain.Queries;
using EntitiesDomain.Responses;

namespace InfrastructureDomain
{
    public interface ISystemRepository
    {
        ITransaction Transaction { get; }

        Task<Response<List<CatalogItem>>> ListCatalogsProcedure(CatalogListQuery query);
        Task<Response<List<MenuOption>>> ListMenuOptionsProcedure(MenuOptionsQuery query);
    }
}
