using DataAccessLayer.Core.UoW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Identity.PermissionManager.Api.Controllers
{
    [Authorize(Policy = "IdentityUser")]
    [EnableCors("AllowAll")]
    public abstract class BaseApiController : Controller
    {
        protected readonly IUnitOfWork uow;
        protected BaseApiController(IUnitOfWork uow) : base()
        {
            this.uow = uow;
        }
    }
}
