using DataAccessLayer.Core.Interfaces.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Identity.PermissionManager.Api.Controllers
{
    [Authorize(Policy = "IdentityUser")]
    [EnableCors("AllowAll")]
    public abstract class BaseApiController : Controller
    {
        protected readonly IUnitOfWork _uow;
        protected BaseApiController(IUnitOfWork uow) : base()
        {
            this._uow = uow;
        }
    }
}
