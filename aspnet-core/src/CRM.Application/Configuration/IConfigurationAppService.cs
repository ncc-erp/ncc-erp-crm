using System.Threading.Tasks;
using CRM.Configuration.Dto;

namespace CRM.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
