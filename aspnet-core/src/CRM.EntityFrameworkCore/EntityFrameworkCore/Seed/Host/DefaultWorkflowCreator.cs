using CRM.Authorization.Roles;
using CRM.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using static CRM.Enums.StatusEnum;

namespace CRM.EntityFrameworkCore.Seed.Host
{
    public class DefaultWorkflowCreator
    {
        private readonly CRMDbContext _context;

        public DefaultWorkflowCreator(CRMDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateSeedDataWorkflow();
        }

        private void CreateSeedDataWorkflow()
        {
            var workflow = _context.Workflows.IgnoreQueryFilters().FirstOrDefault(w => w.WorkflowName == "Invoice Workflow Default");
            if (workflow == null)
            {
                workflow = _context.Workflows.Add(new Entities.Workflow
                {
                    EntityName = "Invoice",
                    WorkflowName = "Invoice Workflow Default",
                    DefaultStatus = (int)InvoiceStatus.Pending,
                    IsActive = true
                }).Entity;
                _context.SaveChanges();
            }

            foreach (InvoiceStatus status in (InvoiceStatus[])Enum.GetValues(typeof(InvoiceStatus)))
            {
                var workflowStatus = _context.WorkflowStatuses.IgnoreQueryFilters().FirstOrDefault(ws => ws.WorkflowId == workflow.Id && ws.Status == (int)status);
                if (workflowStatus == null)
                {
                    _context.WorkflowStatuses.Add(new Entities.WorkflowStatus
                    {
                        WorkflowId = workflow.Id,
                        Status = (int)status,
                        StatusName = Enum.GetName(typeof(InvoiceStatus), status)
                    });
                    _context.SaveChanges();
                }
            }

            foreach (var item in ListWorkflowTransitions)
            {
                var workflowTransition = _context.WorkflowTransitions.IgnoreQueryFilters().FirstOrDefault(w => w.WorkflowId == workflow.Id
                                                                        && w.FromStatus == item.FromStatus && w.ToStatus == item.ToStatus);
                if (workflowTransition == null)
                {
                    item.WorkflowId = workflow.Id;
                    workflowTransition = _context.WorkflowTransitions.Add(item).Entity;
                    _context.SaveChanges();

                    var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Tenants.Admin);
                    var saleRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Tenants.Sale);
                    var pmRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Tenants.PM);
                    var accountantRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == null && r.Name == StaticRoleNames.Tenants.Accountant);

                    if (adminRole == null)
                    {
                        adminRole = _context.Roles.Add(new Role(null, StaticRoleNames.Host.Admin, StaticRoleNames.Host.Admin) { IsStatic = true, IsDefault = true }).Entity;
                        _context.SaveChanges();
                    }
                    if (pmRole == null)
                    {
                        pmRole = _context.Roles.Add(new Role(null, StaticRoleNames.Tenants.PM, StaticRoleNames.Tenants.PM) { IsStatic = true, IsDefault = true }).Entity;
                        _context.SaveChanges();
                    }
                    if (saleRole == null)
                    {
                        adminRole = _context.Roles.Add(new Role(null, StaticRoleNames.Tenants.Sale, StaticRoleNames.Tenants.Sale) { IsStatic = true, IsDefault = true }).Entity;
                        _context.SaveChanges();
                    }
                    if (accountantRole == null)
                    {
                        adminRole = _context.Roles.Add(new Role(null, StaticRoleNames.Tenants.Accountant, StaticRoleNames.Tenants.Accountant) { IsStatic = true, IsDefault = true }).Entity;
                        _context.SaveChanges();
                    }
                    if (workflowTransition.FromStatus == (int)InvoiceStatus.Chasing && workflowTransition.ToStatus == (int)InvoiceStatus.Paid)
                    {
                        _context.TransitionPermissions.Add(new TransitionPermission { RoleId = saleRole.Id, WorkflowTransitionId = workflowTransition.Id });
                        _context.TransitionPermissions.Add(new TransitionPermission { RoleId = accountantRole.Id, WorkflowTransitionId = workflowTransition.Id });
                    }
                    _context.TransitionPermissions.Add(new TransitionPermission { RoleId = adminRole.Id, WorkflowTransitionId = workflowTransition.Id });
                    _context.TransitionPermissions.Add(new TransitionPermission { RoleId = pmRole.Id, WorkflowTransitionId = workflowTransition.Id });

                    if (workflowTransition.FromStatus == (int)InvoiceStatus.Paid && workflowTransition.ToStatus == (int)InvoiceStatus.Chasing)
                    {
                        _context.TransitionPermissions.Add(new TransitionPermission { RoleId = accountantRole.Id, WorkflowTransitionId = workflowTransition.Id });
                    }
                    _context.SaveChanges();
                }
            }

            //WorkFlow client

            var workflowClient = _context.Workflows.IgnoreQueryFilters().FirstOrDefault(w => w.WorkflowName == "Client Workflow Default");
            if (workflowClient == null)
            {
                workflowClient = _context.Workflows.Add(new Entities.Workflow
                {
                    EntityName = Enum.GetName(typeof(EntityDefault), EntityDefault.Client),
                    WorkflowName = "Client Workflow Default",
                    DefaultStatus = (int)ClientStatus.New,
                    IsActive = true
                }).Entity;
                _context.SaveChanges();
            }

            foreach (ClientStatus status in (ClientStatus[])Enum.GetValues(typeof(ClientStatus)))
            {
                var workflowStatus = _context.WorkflowStatuses.IgnoreQueryFilters().FirstOrDefault(ws => ws.WorkflowId == workflowClient.Id && ws.Status == (int)status);
                if (workflowStatus == null)
                {
                    _context.WorkflowStatuses.Add(new Entities.WorkflowStatus
                    {
                        WorkflowId = workflowClient.Id,
                        Status = (int)status,
                        StatusName = Enum.GetName(typeof(ClientStatus), status)
                    });
                    _context.SaveChanges();
                }
            }
        }

        private WorkflowTransition[] ListWorkflowTransitions =
        {
            new WorkflowTransition { FromStatus = (int)InvoiceStatus.Pending, ToStatus = (int)InvoiceStatus.Chasing},
            new WorkflowTransition { FromStatus = (int)InvoiceStatus.Chasing, ToStatus = (int)InvoiceStatus.Pending},
            new WorkflowTransition { FromStatus = (int)InvoiceStatus.Chasing, ToStatus = (int)InvoiceStatus.Fail},
            new WorkflowTransition { FromStatus = (int)InvoiceStatus.Chasing, ToStatus = (int)InvoiceStatus.Paid},
            new WorkflowTransition { FromStatus = (int)InvoiceStatus.Fail, ToStatus = (int)InvoiceStatus.Chasing},
            new WorkflowTransition { FromStatus = (int)InvoiceStatus.Paid, ToStatus = (int)InvoiceStatus.Chasing},
        };
    }
}