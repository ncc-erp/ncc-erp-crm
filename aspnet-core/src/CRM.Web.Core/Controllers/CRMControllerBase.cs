using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace CRM.Controllers
{
    public abstract class CRMControllerBase: AbpController
    {
        protected CRMControllerBase()
        {
            LocalizationSourceName = CRMConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
