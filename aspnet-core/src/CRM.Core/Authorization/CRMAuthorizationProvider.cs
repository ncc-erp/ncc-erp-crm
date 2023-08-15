using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace CRM.Authorization
{
    public class CRMAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            //context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            //context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
            foreach (var permission in SystemPermission.ListPermissions)
            {
                context.CreatePermission(permission.Permission, L(permission.DisplayName), multiTenancySides: permission.MultiTenancySides);
            }
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, CRMConsts.LocalizationSourceName);
        }
    }
}
