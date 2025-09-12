using EntitiesDomain.Entities;
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
        public async Task<Response<List<MenuOption>>> ListMenuOptionsProcedure(MenuOptionsQuery query)
        {
            Response<List<MenuOption>> response = new();
            try
            {
                Response<(List<Option>, List<Actions>)> access = await _dataBase.QueryMultiple<Option, Actions>("sp_ListMenuOptions", query);
                if (response.IsOk(access))
                {
                    response.Code = HttpStatusCode.OK;
                    response.Message = "Query completed successfully";
                    response.Data = GetSubOptions(query.IdOption, access.Data.Item1, access.Data.Item2);
                }
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(ListMenuOptionsProcedure));
            }
            return response;
        }
        private List<MenuOption> GetSubOptions(int idParent, List<Option> options, List<Actions> actions)
        {
            return options
                .Where(m => m.IdParent == idParent)
                .Select(m => new MenuOption
                {
                    IdOption = m.IdOption,
                    OptionName = m.OptionName,
                    Route = m.Route,
                    IdParent = m.IdParent,
                    Icon = m.Icon,
                    Actions = GetActions(m.IdOption, actions),
                    SubOptions = GetSubOptions(m.IdOption, options, actions)
                }).ToList();
        }
        private List<MenuAction> GetActions(int idOption, List<Actions> menuAccessList)
        {
            return menuAccessList
                .Where(m => m.IdOption == idOption)
                .Select(m => new MenuAction
                {
                    IdAction = m.IdAction,
                    ActionName = m.ActionName,
                    ActionCode = m.ActionCode
                }).ToList();
        }
    }
}
