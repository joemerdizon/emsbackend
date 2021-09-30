using Microsoft.AspNetCore.Authorization;

namespace EMS_Backend.Helper
{
    public class RoleAuthorizeAttribute: AuthorizeAttribute
    {

        private string _roles = "superadmin, admin, adminstaff, member";
        public RoleAuthorizeAttribute()
        {
            this.Roles = _roles;

        }
        public RoleAuthorizeAttribute(string policy)
        {
            if (policy != null && policy != "" && policy.Length > 0)
            {
                this.Roles = policy;
            }
            else
            {
                this.Roles = _roles;
            }
            
        }
    }
}
