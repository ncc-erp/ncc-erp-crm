using Abp.Authorization;
using CRM.Authorization.Roles;
using CRM.Authorization.Users;

namespace CRM.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
