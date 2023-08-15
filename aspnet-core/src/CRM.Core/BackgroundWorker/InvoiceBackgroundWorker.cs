using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using CRM.Entities;
using CRM.Extension;
using CRM.IoC;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;
using System.Linq;
using CRM.Authorization.Users;
using Abp.Authorization.Users;

namespace CRM.BackgroundWorker
{
    public class InvoiceBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public IWorkScope WorkScope { get; set; }
        public InvoiceBackgroundWorker(AbpTimer timer) : base(timer)
        {
            WorkScope = IocManager.Instance.Resolve<IWorkScope>();
            Timer.Period = 86400000;
            LocalizationSourceName = CRMConsts.LocalizationSourceName;
        }
        [UnitOfWork]
        protected override void DoWork()
        {
            var now = DateTime.Now;
            //create invoice at first day of month
            if (now.Day == 1)
            {
                var accountant = WorkScope.GetAll<UserRole>().Where(s => s.RoleId == 6).Select(s => s.UserId).FirstOrDefault();
                var listContract = (from c in WorkScope.GetAll<Contract>().Where(s => s.Type == ContractType.ODC || s.Type == ContractType.TNM)
                                    .Where(s => s.EndTime == null || (s.EndTime != null && ((DateTime)s.EndTime).Month >= now.Month))
                                    join p in WorkScope.GetAll<Project>().Where(s => s.Status == ProjectStatus.OnGoing) on c.ProjectId equals p.Id
                                    select new Contract
                                    {
                                        Id = c.Id,
                                        Name = c.Name,
                                        StartTime = c.StartTime,
                                        ContractValue = c.ContractValue,
                                        Currency = c.Currency,
                                        EndTime = c.EndTime,
                                        ProjectId = p.Id,
                                        Type = c.Type
                                    }).ToList();
                long lastInvoiceOrder = 0;
                foreach (var contract in listContract)
                {
                    var invoiceCount = WorkScope.GetAll<Invoice>().Where(i => i.ContractId == contract.Id).Count();
                    invoiceCount++;
                    var lastInvoice = WorkScope.GetAll<Invoice>().LastOrDefault(i => i.ContractId == contract.Id);
                    var currentProject = WorkScope.Get<Project>(contract.ProjectId);
                    if (lastInvoice == null)
                    {
                        WorkScope.Insert<Invoice>(new Invoice
                        {
                            Assignee = accountant,
                            ContractId = contract.Id,
                            InvoiceDate = now.AddDays(-(now.Day)).AddMonths(1),
                            Currency = contract.Currency,
                            ContractMileStoneId = null,
                            Name = $"{currentProject.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                            Order = lastInvoiceOrder + 1,
                            Status = InvoiceStatus.Pending,
                            Type = contract.Type
                        });
                    }
                    else if (now.Month > lastInvoice.InvoiceDate.Month)
                    {
                        if (lastInvoice != default)
                        {
                            lastInvoiceOrder = lastInvoice.Order;
                        }
                        WorkScope.Insert<Invoice>(new Invoice
                        {
                            Assignee = accountant,
                            ContractId = contract.Id,
                            InvoiceDate = now.AddDays(-(now.Day)).AddMonths(1),
                            Currency = contract.Currency,
                            ContractMileStoneId = null,
                            Name = $"{currentProject.Name + invoiceCount + DateTime.Now.Date + invoiceCount}",
                            Order = lastInvoiceOrder + 1,
                            Status = InvoiceStatus.Pending,
                            Type = contract.Type
                        });
                    }
                }
                CurrentUnitOfWork.SaveChanges();
            }
        }
    }
}
