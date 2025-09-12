using ApplicationDomain;
using EntitiesDomain.Queries;
using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CertiPoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly string currentClass = typeof(SystemController).FullName;
        private readonly ISystemBusiness _system;
        public SystemController(ISystemBusiness system)
        {
            _system = system;
        }
        [AllowAnonymous]
        [HttpGet("catalogs_list")]
        public async Task<IActionResult> ListCatalogsController([FromQuery] CatalogListQuery query)
        {
            Response<List<CatalogItem>> response = new();
            try
            {
                response = await _system.ListCatalogsService(query);
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(ListCatalogsController));
            }
            return Ok(response.Result());
        }

        [AllowAnonymous]
        [HttpGet("menu_list")]
        public async Task<IActionResult> ListMenuOptionsController([FromQuery] MenuOptionsQuery query)
        {
            Response<List<MenuOption>> response = new();
            try
            {
                query.IdUser = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                response = await _system.ListMenuOptionsService(query);
            }
            catch (Exception ex)
            {
                response.AddError(ex, currentClass, nameof(ListMenuOptionsController));
            }
            return Ok(response.Result());
        }

    }
}
