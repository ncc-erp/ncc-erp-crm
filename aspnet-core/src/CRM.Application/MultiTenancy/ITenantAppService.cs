using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CRM.MultiTenancy.Dto;

namespace CRM.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedTenantResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}

