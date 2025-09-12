using EntitiesDomain.Queries;
using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using InfrastructureDomain;
using System.Data;
using System.Net;

namespace Infrastructure
{
    public class SystemRepository : ISystemRepository
    {
        public ITransaction Transaction { get; }
        private readonly string currentClass = typeof(SystemRepository).FullName;
        private readonly IDataBase _dataBase;

        public SystemRepository(IDataBase dataBase)
        {
            Transaction = _dataBase = dataBase;
        }
        public async Task<Response<List<CatalogItem>>> ListCatalogsProcedure(CatalogListQuery query)
        {
            Response<List<CatalogItem>> response = new();
            try
            {
                response = await _dataBase.Query<CatalogItem>("sp_ListCatalogs", query);
                if (response.IsOk())
                {
                    ILookup<int?, CatalogItem> lookup = response.Data.ToLookup(x => x.IdParent);

                    response.Code = HttpStatusCode.OK;
                    response.Message = "Query completed successfully";
                    response.Data = GetSubItems(null,lookup);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(ListCatalogsProcedure));
            }
            return response;
        }
        private List<CatalogItem> GetSubItems(int? parentId, ILookup<int?, CatalogItem> lookup)
        {
            List<CatalogItem> subItems = lookup[parentId].ToList();

            foreach (var subItem in subItems)
            {
                subItem.SubItems = GetSubItems(subItem.Id, lookup);
            }

            return subItems;
        }
    }
}
