using Abp.UI;
using CRM.Entities;
using CRM.Extension;
using CRM.Paging;
using CRM.Projects.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CRM.Authorization.Users;
using Nito.AsyncEx;
using CRM.Enums;
using CRM.APIWorkflowController;
using static CRM.Enums.StatusEnum;

namespace CRM.Projects
{
    public class ProjectAppService : CRMAppServiceBase
    {
        private WorkflowControllerAppService _workflowControllerAppService;
        public ProjectAppService(WorkflowControllerAppService workflowControllerAppService)
        {
            _workflowControllerAppService = workflowControllerAppService;
        }
        [HttpPost]
        public async Task<long> Save(SaveProjectDto input)
        {
            // create
            if (input.Id <= 0)
            {
                var item = ObjectMapper.Map<Project>(input);
                input.Id = await WorkScope.InsertAndGetIdAsync<Project>(item);
                // add project assignee
                foreach (var userId in input.Users)
                {
                    await WorkScope.InsertAsync<ProjectAssignee>(new ProjectAssignee
                    {
                        ProjectId = input.Id,
                        UserId = userId
                    });
                }
                await CurrentUnitOfWork.SaveChangesAsync();
            }
            // update
            else
            {
                var currentProject = await WorkScope.GetAsync<Project>(input.Id);
                if (currentProject.Name != input.Name)
                {
                    var NameOfEntity = WorkScope.GetAll<EntityAssignment>().Where(s => s.EntityId == input.Id && s.EntityType == EntityDefault.Project);
                    foreach (var i in NameOfEntity)
                    {
                        i.NameOfEntity = input.Name;
                    }
                    await WorkScope.UpdateRangeAsync(NameOfEntity);
                }
                ObjectMapper.Map<SaveProjectDto, Project>(input, currentProject);
                await WorkScope.UpdateAsync(currentProject);
                _workflowControllerAppService.ChangeStatusProject(input.Id, input.Status);
                await CurrentUnitOfWork.SaveChangesAsync();
                //update project assignee
                var oldUsers = await WorkScope.GetAll<ProjectAssignee>().Where(s => s.ProjectId == input.Id)
                    .Select(s => new { s.UserId, s.Id }).ToListAsync();
                var oldUserIds = oldUsers.Select(s => s.UserId);
                //add new
                var userNeedToAdd = input.Users.Except(oldUserIds);
                foreach (var userId in userNeedToAdd)
                {
                    await WorkScope.InsertAsync<ProjectAssignee>(new ProjectAssignee
                    {
                        ProjectId = input.Id,
                        UserId = userId
                    });
                }
                //delete old
                var userNeedToDelete = oldUserIds.Except(input.Users);
                var projectAssigneeNeedToDelete = oldUsers.Where(s => userNeedToDelete.Contains(s.UserId))
                    .Select(s => s.Id);
                foreach (var projectAssigneeId in projectAssigneeNeedToDelete)
                {
                    await WorkScope.DeleteAsync<ProjectAssignee>(projectAssigneeId);
                }
            }
            return input.Id;
        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var count = await WorkScope.GetAll<Contract>().CountAsync(s => s.ProjectId == id);
            var currentProject = await WorkScope.GetAsync<Project>(id);
            if (count > 0)
            {
                throw new UserFriendlyException(String.Format("Project {0} has Contract data!!!", currentProject.Name));
            }           
            if (currentProject == null)
            {
                throw new UserFriendlyException("Không tìm thấy Project");
            }
            await WorkScope.DeleteAsync<Project>(id);
            var listProjectAssignee = await WorkScope.GetAll<ProjectAssignee>().Where(s => s.ProjectId == id)
                .Select(s => s.Id).ToListAsync();
            foreach (var deleteId in listProjectAssignee)
            {
                await WorkScope.DeleteAsync<ProjectAssignee>(id);
            }
        }

        [HttpGet]
        public async Task<ProjectDetailDto> GetProject(long id)
        {
            var currentProject = await WorkScope.GetAsync<Project>(id);
            var currentClient = await WorkScope.GetAsync<Client>(currentProject.ClientId);
            var listUserAssignees = await WorkScope.GetAll<ProjectAssignee>().Include(pa => pa.User).Where(pa => pa.ProjectId == currentProject.Id).ToListAsync();
            var listUserName = listUserAssignees.Select(ua => ua.User.Name).ToList();
            var currentContracts = await WorkScope.GetAll<Contract>().Where(cc => cc.ProjectId == currentProject.Id).ToListAsync();
            var currentContractIds = currentContracts.Select(c => c.Id).ToList();
            var currentContractMileStones = await WorkScope.GetAll<ContractMileStone>().Where(cm => currentContractIds.Contains(cm.ContractId)).ToListAsync();
            var currentContractInvoice = await WorkScope.GetAll<Invoice>().Where(i => currentContractIds.Contains(i.ContractId)).ToListAsync();
            var currentInvoiceUser = await WorkScope.GetAll<InvoiceUser>().Include(i => i.User).Where(i => currentContractInvoice.Select(x => x.Id).Contains(i.InvoiceId)).ToListAsync();

                   var projects = (from ua in listUserAssignees
                                select new ProjectDetailDto
                                {
                                    ClientId = currentProject.ClientId,
                                    Id = currentProject.Id,
                                    Client = currentClient.Name,
                                    ProjectName = currentProject.Name,
                                    Description = currentProject.Description,
                                    ProjectCode = currentProject.Code,
                                    UserName = listUserName,
                                    ProjectStatus = currentProject.Status,
                                    ProjectType = currentProject.Type,
                                    ProjectContractDetails = currentContracts.Select(cc => new ProjectContractDetailDto
                                    {
                                        ContractCurrency = cc.Currency,
                                        ContractId = cc.Id,
                                        ProjectId = cc.ProjectId,
                                        StartTime = cc.StartTime,
                                        EndTime = cc.EndTime,
                                        ContractStatus = cc.Status,
                                        ContractType = cc.Type,
                                        ContractValue = cc.ContractValue,
                                        ContractName = cc.Name,
                                        ContractMileStones = cc.Type == StatusEnum.ContractType.FixedPrice ? currentContractMileStones.Where(x => x.ContractId == cc.Id).Select(ccm => new ContractMileStoneDto
                                        {
                                            MileStoneId = ccm.Id,
                                            Percentage = ccm.Percentage,
                                            MileStoneValue = ccm.Value,
                                            InvoiceStatus = currentContractInvoice.First(cci => cci.ContractId == ccm.ContractId && cci.ContractMileStoneId == ccm.Id).Status,
                                            Description = ccm.Description,
                                            MileStone = ccm.Name,
                                            MileStoneDate = ccm.MileStoneDate
                                        })
                                          : null,
                                        ContractInVoices = (cc.Type == StatusEnum.ContractType.TNM || cc.Type == StatusEnum.ContractType.ODC) ? currentContractInvoice.Where(x => x.ContractId == cc.Id).Select(cci => new ContractInVoiceDto
                                        {
                                            InvoiceId = cci.Id,
                                            InvoiceStatus = cci.Status,
                                            PeopleInCharges = currentInvoiceUser.Where(x => x.InvoiceId == cci.Id).Select(pip => new PeopleInCharge
                                            {
                                                UserId = pip.UserId,
                                                InvoiceUserId = pip.Id,
                                                ManMonth = pip.ManMonth,
                                                Position = pip.Position,
                                                Rate = pip.Rate,
                                                UserName = pip.User.UserName
                                            }).ToList(),
                                            Time = cci.InvoiceDate
                                        }) : null
                                    }),
                                    AssigneeId = listUserAssignees.Select(us => us.User.Id).ToList()
        }
                             ).FirstOrDefault();
            return projects;
        }
        [HttpPost]
        public async Task<GridResult<ViewProjectDto>> GetAllPaging(GridParam input)
        {
            var projects = from p in WorkScope.GetAll<Project>()
                           join c in WorkScope.GetAll<Client>() on p.ClientId equals c.Id
                           select new ViewProjectDto
                           {
                               Id = p.Id,
                               ClientId = p.ClientId,
                               ClientName = c.Name,
                               Name = p.Name,
                               Status = p.Status,
                               Type = p.Type
                           };
            return await projects.GetGridResult(projects, input);
        }
    }
}
