using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.UI;
using CRM.Authorization.Users;
using CRM.Dashboard.Dto;
using CRM.Entities;
using CRM.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.Dashboard
{
    [AbpAuthorize]
    public class DashboardAppService : CRMAppServiceBase
    {
        ///
        [HttpGet]
        public async Task<DashboardDto> GetLatestInformation(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                if (startDate.Value.Date > endDate.Value.Date)
                {
                    throw new UserFriendlyException("Start Date must be less than End Date");
                }
            }

            //get client
            var clients = WorkScope.GetAll<Client>()
                .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
                .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
                .GroupBy(x => x.Status)
                .Select(z => new StatusReport
                {
                    Name = Enum.GetName(typeof(ClientStatus), z.Key),
                    Value = z.Key.ToString(),
                    Total = z.Count()
                }).ToList();


            //get project
            var projects = WorkScope.GetAll<Project>()
                .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
                .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
                .GroupBy(x => x.Status)
                .Select(z => new StatusReport
                {
                    Name = Enum.GetName(typeof(ProjectStatus), z.Key),
                    Value = z.Key.ToString(),
                    Total = z.Count()
                }).ToList();

            //get invoice
            var invoices = WorkScope.GetAll<Deal>()
                .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
                .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
                .GroupBy(x => x.Status)
                .Select(z => new StatusReport
                {
                    Name = Enum.GetName(typeof(DealStatus), z.Key),
                    Value = z.Key.ToString(),
                    Total = z.Count()
                }).ToList();

            //get account
            var accountsCount = WorkScope.GetAll<User>()
                .WhereIf(startDate.HasValue, x => x.CreationTime.Date >= startDate.Value.Date)
                .WhereIf(endDate.HasValue, x => x.CreationTime.Date <= endDate.Value.Date)
                .GroupBy(x => x.IsActive)
                .Select(z => new AccountReport
                {
                    Name = z.Key == true ? "Activated" : "NotActivated",
                    Value = z.Key,
                    Total = z.Count()
                }).ToList();

            return new DashboardDto
            {
                Clients = clients,
                Projects = projects,
                Invoices = invoices,
                AccountsCount = accountsCount,
            };

        }
    }
}
