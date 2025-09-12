using ApplicationDomain;
using EntitiesDomain.Queries;
using EntitiesDomain.Responses;
using EntitiesDomain.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    }
}
