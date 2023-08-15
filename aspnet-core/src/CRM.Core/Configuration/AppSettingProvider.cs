using System.Collections.Generic;
using Abp.Configuration;

namespace CRM.Configuration
{
    public class AppSettingProvider : SettingProvider
    {
        public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
        {
            return new[]
            {
                new SettingDefinition(AppSettingNames.UiTheme, "red", scopes: SettingScopes.Application | SettingScopes.Tenant | SettingScopes.User, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.ClientAppId,"",scopes:SettingScopes.Application| SettingScopes.Tenant),
                new SettingDefinition(AppSettingNames.EmailTemplate.InvoiceAfter10dayEmail, "", scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.EmailTemplate.InvoiceBefore10dayEmail, "", scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
                new SettingDefinition(AppSettingNames.EmailTemplate.InvoiceEmailHeader, "", scopes: SettingScopes.Application | SettingScopes.Tenant, isVisibleToClients: true),
            };
        }
    }
}
