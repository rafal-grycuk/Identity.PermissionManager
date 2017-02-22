using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Identity.PermissionManager.Utilities.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetErrorsString(this IEnumerable<IdentityError> errors)
        {
            var resultString = string.Empty;
            errors.ToList().ForEach(e =>
            {
                resultString += " Error code:" + e.Code + "; " + e.Description + "\n";
            });
            return resultString;
        }
    }
}
