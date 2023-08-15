using CRM.Clients.Dto;
using CRM.Entities;
using CRM.IoC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.UI;
using CRM.Paging;
using CRM.Extension;
using CRM.WorkFlows;
using static CRM.Enums.StatusEnum;
using Microsoft.EntityFrameworkCore.Internal;
using Abp.Authorization;
using CRM.APIWorkflowController;

namespace CRM.Clients
{
    public class ClientAppService : CRMAppServiceBase
    {
        private readonly WorkflowAppService _workflowAppService;
        private WorkflowControllerAppService _workflowControllerAppService;
        public ClientAppService(WorkflowAppService workflowAppService, WorkflowControllerAppService workflowControllerAppService)
        {
            _workflowAppService = workflowAppService;
            _workflowControllerAppService = workflowControllerAppService;
        }

        [HttpPost]
        public async Task<ClientDetailDto> Save(ClientDetailDto input)
        {
            var client = ObjectMapper.Map<Client>(input);
            if (input.Id>0)
            {
                var NameOfEntity = WorkScope.GetAll<EntityAssignment>().Where(s => s.EntityId == input.Id && s.EntityType == EntityDefault.Client);
                foreach (var i in NameOfEntity)
                {
                    i.NameOfEntity = input.Name;
                }
                await WorkScope.UpdateRangeAsync(NameOfEntity);
            }
            var clientId = await WorkScope.GetRepo<Client>().InsertOrUpdateAndGetIdAsync(client);
            await CurrentUnitOfWork.SaveChangesAsync();
            var currentClient = await WorkScope.GetAsync<Client>(clientId);
            if (currentClient == null)
            {
                throw new UserFriendlyException("Không thành công");
            }
            return new ClientDetailDto
            {
                Id = currentClient.Id,
                Country = currentClient.Country,
                Description = currentClient.Description,
                Mail = currentClient.Mail,
                Name = currentClient.Name,
                Phone = currentClient.Phone,
                Type = currentClient.Type,
                Website = currentClient.Website,
                Status = currentClient.Status

            };
        }

        public async Task ChangeStatusClient(ClientStatusDto input)
        {
            var currentClient = await WorkScope.GetAsync<Client>(input.Id);
            if (currentClient == null)
            {
                throw new UserFriendlyException("Không thành công");
            }
            currentClient.Status = input.Status;
            await CurrentUnitOfWork.SaveChangesAsync();

        }
        [HttpDelete]
        public async Task Delete(long id)
        {
            var count = await WorkScope.GetAll<Deal>().CountAsync(s => s.ClientId == id);
            var currentClient = await WorkScope.GetAsync<Client>(id);
            if (count > 0)
            {
                throw new UserFriendlyException(String.Format("Client {0} has Deal data!!!", currentClient.Name));
            }
            count = await WorkScope.GetAll<Project>().CountAsync(s => s.ClientId == id);
            if (count > 0)
            {
                throw new UserFriendlyException(String.Format("Client {0} has Project data!!!", currentClient.Name));
            }
            //count = await WorkScope.GetAll<ClientContact>().CountAsync(s => s.ClientId == id);
            //if (count > 0)
            //{
            //    throw new UserFriendlyException(String.Format("Client %s has Contact data!!!", currentClient.Name));
            //}
            if (currentClient == null)
            {
                throw new UserFriendlyException("Không tìm thấy khách hàng");
            }
            await WorkScope.DeleteAsync<Client>(id);
        }
        [HttpGet]
        public async Task<ClientDetailDto> GetById(long id)
        {
            try
            {
                return await (from c in WorkScope.GetAll<Client>().Where(s => s.Id == id)
                              join d in WorkScope.GetAll<Deal>() on c.Id equals d.ClientId into t1
                              join p in WorkScope.GetAll<Project>() on c.Id equals p.ClientId into t2
                              select new ClientDetailDto
                              {
                                  Id = c.Id,
                                  Country = c.Country,
                                  Description = c.Description,
                                  Mail = c.Mail,
                                  Name = c.Name,
                                  Phone = c.Phone,
                                  Type = c.Type,
                                  Website = c.Website,
                                  Status = c.Status,
                                  RegionId = c.RegionId,
                                  RegionName = c.Region.Name,
                                  Deals = t1.Select(s => new DealClientDto
                                  {
                                      Id = s.Id,
                                      Amount = s.Amount,
                                      ClosingDate = s.ClosingDate,
                                      Description = s.Description,
                                      LoseReason = s.LoseReason,
                                      Name = s.Name,
                                      OwnerId = s.OwnerId,
                                      OwnerName = s.Owner.Name,
                                      Status = s.Status,
                                      WinReason = s.WinReason
                                  }).ToList(),
                                  Projects = t2.Select(s => new ProjectClientDto
                                  {
                                      Id = s.Id,
                                      Status = s.Status,
                                      Name = s.Name,
                                      Description = s.Description,
                                      Code = s.Code,
                                      Type = s.Type
                                  }).ToList()
                              }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet]
        public async Task<List<ClientDto>> GetClientForProject()
        {
            var clients = await WorkScope.GetAll<Client>().Select(c => new ClientDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return clients;
        }
        [AbpAuthorize, HttpPost]
        public async Task<GridResult<ClientDetailDto>> GetAllPaging(GridParam input)
        {
            var workflowTransitions = await _workflowAppService.GetWorkflowTransition((int)EntityDefault.Client);
            var clients = WorkScope.GetAll<Client>().Select(s => new ClientDetailDto
            {
                Id = s.Id,
                Country = s.Country,
                Description = s.Description,
                Mail = s.Mail,
                Name = s.Name,
                Phone = s.Phone,
                Type = s.Type,
                Website = s.Website,
                Status = s.Status,
                ClientWorkflowTransition = new ClientWorkflowTransitionDto
                {
                    CanChangeToNew = s.Status != ClientStatus.New && workflowTransitions.Any(wt => wt.FromStatus == (int)s.Status && wt.ToStatus == (int)ClientStatus.New && wt.CanChange),
                    CanChangeToRegularContact = s.Status != ClientStatus.RegularContact && workflowTransitions.Any(wt => wt.FromStatus == (int)s.Status && wt.ToStatus == (int)ClientStatus.RegularContact && wt.CanChange),
                    CanChangeToInactiveContact = s.Status != ClientStatus.InactiveContact && workflowTransitions.Any(wt => wt.FromStatus == (int)s.Status && wt.ToStatus == (int)ClientStatus.InactiveContact && wt.CanChange)
                }
            });
            return await clients.GetGridResult(clients, input);
        }

        [HttpGet]
        public async Task<List<RegionDto>> GetDropdownRegion()
        {
            var listRegions = await WorkScope.GetAll<Region>()
                                .Select(x => new RegionDto 
                                { 
                                 RegionId = x.Id, 
                                 RegionName = x.Name
                                }).OrderBy(x => x.RegionName).ToListAsync();
            return listRegions;
        }
    }
}
