using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using CRM.Configuration.Dto;

namespace CRM.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : CRMAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
