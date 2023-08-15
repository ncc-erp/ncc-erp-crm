using Abp;
using Abp.Authorization;
using Abp.Net.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CRM.EmailSettings.Dto
{
    public class EmailSettingAppService : CRMAppServiceBase
    {
        public async Task<MailSettingDto> Get()
        {
            return new MailSettingDto
            {
                FromDisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                Host = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                Password = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password),
                Port = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Port),
                UserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                EnableSsl = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.EnableSsl),
                DefaultFromAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress)
            };
        }

        public async Task<MailSettingDto> Change(MailSettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromDisplayName, input.FromDisplayName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Host, input.Host);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Password, input.Password);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.Port, input.Port);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UserName, input.UserName);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.DefaultFromAddress, input.DefaultFromAddress);
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.UseDefaultCredentials, "false");
            await SettingManager.ChangeSettingForApplicationAsync(EmailSettingNames.Smtp.EnableSsl, input.EnableSsl);
            return input;
        }
    }

}
