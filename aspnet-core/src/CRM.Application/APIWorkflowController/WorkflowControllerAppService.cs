using Abp.Domain.Entities;
using Abp.UI;
using CRM.APIWorkflowController.Dto;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;

namespace CRM.APIWorkflowController
{
    public class WorkflowControllerAppService : CRMAppServiceBase
    {
        public async Task<object> GetAllStatus([Required]string entityName)
        {
            var workflowId = WorkScope.All<Workflow>().FirstOrDefault(s => s.EntityName.Contains(entityName))?.Id;

            var workflowStatus = WorkScope.All<WorkflowStatus>().Where(s => s.WorkflowId == workflowId).Select(s => new { s.Status, s.StatusName }).ToList();

            var workflowTransition = WorkScope.All<WorkflowTransition>()
                .Join(WorkScope.All<TransitionPermission>(),wt => wt.Id,tp => tp.WorkflowTransitionId,(wt,tp) => new 
                {
                    WorkflowTransitionId = wt.Id,
                    FromStatus = wt.FromStatus,
                    ToStatus = wt.ToStatus,
                    WorkflowId = wt.WorkflowId,
                    RoleId = tp.RoleId
                }).Where(s => s.WorkflowId == workflowId).ToList();

            return new 
            {
                ListStatus = workflowStatus,
                Workflow = workflowTransition
            };
        }
        public void ChangeStatusInvoice(long id, InvoiceStatus status, string entityName = "Invoice")
        {
            
            var workflowId = WorkScope.All<Workflow>().FirstOrDefault(s => s.EntityName.Contains(entityName))?.Id;
           

            var invoices = WorkScope.All<Invoice>().Where(s => s.Id == id).FirstOrDefault();

            var invoiceStatus = (int) invoices.Status;

            var listToStatus = WorkScope.All<WorkflowTransition>()
                                              .Where(s => s.WorkflowId == workflowId && s.FromStatus == invoiceStatus)
                                              .Select(s => s.ToStatus).ToList();

            if(!listToStatus.Contains((int)status))
            {
                throw new UserFriendlyException("Sai trạng thái");
            }
            else
            {
                invoices.Status = status;
                WorkScope.Update(invoices);
            }
        }

        public void ChangeStatusClient(long id, ClientStatus status, string entityName = "Client")
        {

            var workflowId = WorkScope.All<Workflow>().FirstOrDefault(s => s.EntityName.Contains(entityName))?.Id;


            var clients = WorkScope.All<Client>().Where(s => s.Id == id).FirstOrDefault();

            var clientsStatus = (int)clients.Status;

            var listToStatus = WorkScope.All<WorkflowTransition>()
                                              .Where(s => s.WorkflowId == workflowId && s.FromStatus == clientsStatus)
                                              .Select(s => s.ToStatus).ToList();

            if (!listToStatus.Contains((int)status))
            {
                throw new UserFriendlyException("Sai trạng thái");
            }
            else
            {
                clients.Status = status;
                WorkScope.Update(clients);
            }
        }

        public void ChangeStatusDeal(long id, DealStatus status, string entityName = "Deal")
        {

            var workflowId = WorkScope.All<Workflow>().FirstOrDefault(s => s.EntityName.Contains(entityName))?.Id;


            var deals = WorkScope.All<Deal>().Where(s => s.Id == id).FirstOrDefault();

            var dealsStatus = (int)deals.Status;

            var listToStatus = WorkScope.All<WorkflowTransition>()
                                              .Where(s => s.WorkflowId == workflowId && s.FromStatus == dealsStatus)
                                              .Select(s => s.ToStatus).ToList();

            if (!listToStatus.Contains((int)status))
            {
                throw new UserFriendlyException("Sai trạng thái");
            }
            else
            {
                deals.Status = status;
                WorkScope.Update(deals);
            }
        }

        public void ChangeStatusProject(long id, ProjectStatus status, string entityName = "Project")
        {

            var workflowId = WorkScope.All<Workflow>().FirstOrDefault(s => s.EntityName.Contains(entityName))?.Id;


            var projects = WorkScope.All<Project>().Where(s => s.Id == id).FirstOrDefault();

            var projectsStatus = (int)projects.Status;

            var listToStatus = WorkScope.All<WorkflowTransition>()
                                              .Where(s => s.WorkflowId == workflowId && s.FromStatus == projectsStatus)
                                              .Select(s => s.ToStatus).ToList();

            if (!listToStatus.Contains((int)status))
            {
                throw new UserFriendlyException("Sai trạng thái");
            }
            else
            {
                projects.Status = status;
                WorkScope.Update(projects);
            }
        }
    }
}
