using Abp.Authorization;
using CRM.Configuration;
using CRM.SingleSignOnSetting.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.SingleSignOnSetting
{
    [AbpAuthorize]
    public class SingleSignOnSettingAppService : CRMAppServiceBase
    {
        public async Task<SingleSignOnSettingDto> Get()
        {
            return new SingleSignOnSettingDto
            {
                ClientAppId = await SettingManager.GetSettingValueAsync(AppSettingNames.ClientAppId),
            };
        }
        
        public async Task<SingleSignOnSettingDto> Change(SingleSignOnSettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(AppSettingNames.ClientAppId, input.ClientAppId);
            return input;
        }
    }
}
