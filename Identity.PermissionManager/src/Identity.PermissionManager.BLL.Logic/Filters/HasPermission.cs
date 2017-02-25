using System.Linq;
using System.Security.Claims;
using Identity.PermissionManager.BLL.Logic.PermissionManager;
using Identity.PermissionManager.BLL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Identity.PermissionManager.BLL.Logic.Filters
{
    public class HasPermissionAttribute : ActionFilterAttribute
    {
        private string[] _permissions;
        private PermissionOperator _permissionOperator;
        private PermissionManager<User, Role, int> permissionManager;

        public HasPermissionAttribute(PermissionOperator permissionOperator, params string[] permissions)
        {
            this._permissions = permissions;
            this._permissionOperator = permissionOperator;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.permissionManager =
                filterContext.HttpContext.RequestServices.GetService(typeof(PermissionManager<User, Role, int>)) as
                    PermissionManager<User, Role, int>;

            base.OnActionExecuting(filterContext);
            var authenticatedUser =
                ((ClaimsIdentity) filterContext.HttpContext.User?.Identity)?.Claims.ToList()
                .Find(claim => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");
            if (!this.permissionManager.CheckPermissions(this._permissionOperator, this._permissions, authenticatedUser.Value))
            {
                filterContext.Result = new UnauthorizedResult();
            }

        }
    }
}
