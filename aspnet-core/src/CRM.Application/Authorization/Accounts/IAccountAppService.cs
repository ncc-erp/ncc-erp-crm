using System.Threading.Tasks;
using Abp.Application.Services;
using CRM.Authorization.Accounts.Dto;

namespace CRM.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
