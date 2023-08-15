using System.Threading.Tasks;
using Abp.Application.Services;
using CRM.Sessions.Dto;

namespace CRM.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
