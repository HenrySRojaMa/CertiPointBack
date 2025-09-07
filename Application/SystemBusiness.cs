using ApplicationDomain;
using EntitiesDomain.Queries;
using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using InfrastructureDomain;

namespace Application
{
    public class SystemBusiness : ISystemBusiness
    {
        private readonly string currentClass = typeof(SystemBusiness).FullName;
        private readonly ISystemRepository _system;
        public SystemBusiness(ISystemRepository system)
        {
            _system = system;
        }
        public async Task<Response<List<CatalogItem>>> ListCatalogsService(CatalogListQuery query)
        {
            Response<List<CatalogItem>> response = new();
            try
            {
                response = await _system.ListCatalogsProcedure(query);
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(ListCatalogsService));
            }
            return response;
        }
    }
}
