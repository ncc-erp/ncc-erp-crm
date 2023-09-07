using Abp.UI;
using CRM.APIAssignments.Assignments.Dto;
using CRM.Entities;
using CRM.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CRM.Enums.StatusEnum;
using System.Linq;
using CRM.Paging;
using CRM.Authorization.Users;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Extensions;
using Abp.Authorization;
using CRM.Uitls;

namespace CRM.APIAssignments.Assignments
{
    [AbpAuthorize]
    public class AssignmentAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<CreateAssignmentDto> Save(CreateAssignmentDto input)
        {
            if (input.Id <= 0)
            {
                var assignment = new Assignment
                {
                    Name = input.Name,
                    Status = input.Status,
                    IsActive = input.IsActive,
                    Deadline = input.Deadline,
                    Priority = input.Priority,
                    Description = input.Description
                };
                long assignmentId = await WorkScope.InsertAndGetIdAsync<Assignment>(assignment);
                input.Id = assignmentId;
                string nameOfEntity="";
                foreach (var u in input.EntityAssignmentDtos)
                {
                    switch (u.EntityType)
                    {
                        case EntityDefault.Invoice:
                            if (!await WorkScope.GetAll<Invoice>().AnyAsync(s => s.Id == u.EntityId))
                            {
                                throw new UserFriendlyException(String.Format("Invoid Id {0} isn't exist", u.EntityId));
                            }
                            nameOfEntity = (await WorkScope.GetAsync<Invoice>(u.EntityId)).Name;
                            break;
                        case EntityDefault.Client:
                            if (!await WorkScope.GetAll<Client>().AnyAsync(s => s.Id == u.EntityId))
                            {
                                throw new UserFriendlyException(String.Format("Client Id {0} isn't exist", u.EntityId));
                            }
                            nameOfEntity = (await WorkScope.GetAsync<Client>(u.EntityId)).Name;
                            break;
                        case EntityDefault.Contract:
                            if (!await WorkScope.GetAll<Contract>().AnyAsync(s => s.Id == u.EntityId))
                            {
                                throw new UserFriendlyException(String.Format("Contract Id {0} isn't exist", u.EntityId));
                            }
                            nameOfEntity = (await WorkScope.GetAsync<Contract>(u.EntityId)).Name;
                            break;
                        case EntityDefault.Deal:
                            if (!await WorkScope.GetAll<Deal>().AnyAsync(s => s.Id == u.EntityId))
                            {
                                throw new UserFriendlyException(String.Format("Deal Id {0} isn't exist", u.EntityId));
                            }
                            nameOfEntity = (await WorkScope.GetAsync<Deal>(u.EntityId)).Name;
                            break;
                        case EntityDefault.Project:
                            if (!await WorkScope.GetAll<Project>().AnyAsync(s => s.Id == u.EntityId))
                            {
                                throw new UserFriendlyException(String.Format("Project Id {0} isn't exist", u.EntityId));
                            }
                            nameOfEntity = (await WorkScope.GetAsync<Project>(u.EntityId)).Name;
                            break;
                        case EntityDefault.Contact:
                            if(!await WorkScope.GetAll<Project>().AnyAsync(s => s.Id == u.EntityId))
                            {
                                throw new UserFriendlyException(String.Format($"Contact Id {u.EntityId} isn't exist"));
                            }
                            nameOfEntity = (await WorkScope.GetAsync<Contact>(u.EntityId)).Name;
                            break;
                    }
                    var ena = new EntityAssignment
                    {
                        AssignmentId = assignmentId,
                        EntityId = u.EntityId,
                        EntityType = u.EntityType,
                        EntityName = Enum.GetName(u.EntityType.GetType(), u.EntityType),
                        NameOfEntity= nameOfEntity

                    };
                    await WorkScope.InsertAsync<EntityAssignment>(ena);
                }
                foreach (var u in input.UserIds)
                {
                    var userAssignment = new UserAssignment
                    {
                        UserId = u,
                        AssignmentId = assignmentId
                    };
                    await WorkScope.InsertAsync<UserAssignment>(userAssignment);
                }
            }
            else
            {
                var assignment = await WorkScope.GetAll<Assignment>().Where(s => s.Id == input.Id).FirstOrDefaultAsync();
                assignment.IsActive = input.IsActive;
                assignment.Name = input.Name;
                assignment.Status = input.Status;
                assignment.Deadline = input.Deadline;
                assignment.Description = input.Description;
                assignment.Priority = input.Priority;
                await WorkScope.UpdateAsync<Assignment>(assignment);

                //delete, insert EntityAssignment
                var currentEntityAssignment = await WorkScope.GetAll<EntityAssignment>().Where(s => s.AssignmentId == input.Id).ToListAsync();
                var newEntityAssignment = input.EntityAssignmentDtos;
                var deleteEntityAssignment = currentEntityAssignment
                    .Where(s => !newEntityAssignment.Select(e => new { e.EntityId, e.EntityType })
                    .Contains(new { s.EntityId, s.EntityType })).ToList();

                var insertEntityAssignments = newEntityAssignment
                   .Where(s => !currentEntityAssignment.Select(c => new { c.EntityId, c.EntityType })
                   .Contains(new { s.EntityId, s.EntityType })).ToList();
                foreach (var u in insertEntityAssignments)
                {
                    var entityAssignment = new EntityAssignment
                    {
                        EntityId = u.EntityId,
                        AssignmentId = input.Id,
                        EntityType = u.EntityType,
                        EntityName = Enum.GetName(u.EntityType.GetType(), u.EntityType)
                    };
                    await WorkScope.InsertAsync<EntityAssignment>(entityAssignment);
                }
                foreach (var d in deleteEntityAssignment)
                {
                    await WorkScope.DeleteAsync<EntityAssignment>(d.Id);
                }
                //delete, insert User Assignment
                var currentUser = await WorkScope.GetAll<UserAssignment>().Where(s => s.AssignmentId == input.Id).ToListAsync();
                var newCurrentUser = input.UserIds;
                var deleteUser = currentUser.Where(s => !newCurrentUser.Contains(s.UserId)).ToList();
                var insertUser = newCurrentUser.Except(currentUser.Select(u => u.UserId));
                foreach (var u in insertUser)
                {
                    var userA = new UserAssignment
                    {
                        AssignmentId = input.Id,
                        UserId = u
                    };
                    await WorkScope.InsertAsync<UserAssignment>(userA);
                }
                foreach (var d in deleteUser)
                {
                    await WorkScope.DeleteAsync<UserAssignment>(d.Id);
                }
            }

            return input;
        }

        [HttpPost]
        public async Task<GridResult<GetAssignmentDto>> GetAllPaging(GetAllAssignmentInputDto input)
        {
            if (input.StartDate.HasValue && input.EndDate.HasValue)
            {
                if (input.StartDate.Value > input.EndDate.Value)
                {
                    throw new UserFriendlyException(CommonUtils.ValidationDateRange);
                }
            }

            var query = (from a in WorkScope.GetAll<Assignment>()
                         join e in WorkScope.GetAll<EntityAssignment>() on a.Id equals e.AssignmentId
                         join ua in WorkScope.GetRepo<UserAssignment>().GetAllIncluding(s => s.User, L => L.User2) on a.Id equals ua.AssignmentId
                         where (!input.StartDate.HasValue || a.Deadline >= input.StartDate)
                         where (!input.EndDate.HasValue || a.Deadline <= input.EndDate)
                         where (input.EntityType == null || e.EntityType == input.EntityType)
                         where (input.Priority == null || a.Priority == input.Priority)
                         where (input.Status == null || a.Status == input.Status)
                         where (input.Param.SearchText.IsNullOrWhiteSpace() || a.Name.Contains(input.Param.SearchText, StringComparison.OrdinalIgnoreCase))

                         select new GetAssignmentDto
                         {
                             Id = a.Id,
                             IsActive = a.IsActive,
                             Name = a.Name,
                             Status = a.Status,
                             Priority = a.Priority,
                             Deadline = a.Deadline,
                             Description = a.Description,
                             CreatorUserId = a.CreatorUserId,
                             EntityId = e.EntityId,
                             EntityTypeName = e.EntityName,
                             EntityType = e.EntityType,
                             UserId = ua.UserId,
                             EntityName = e.NameOfEntity,
                             CreatorUserName = $"{ua.User2.Surname} {ua.User2.Name}",
                             FullName = $"{ua.User.Surname} {ua.User.Name}"
                         }).WhereIf(!input.SearchUser.IsNullOrWhiteSpace(), s => s.FullName.Contains(input.SearchUser, StringComparison.OrdinalIgnoreCase))
                        .WhereIf(!input.SearchEntity.IsNullOrWhiteSpace(), s => s.EntityName.Contains(input.SearchEntity, StringComparison.OrdinalIgnoreCase))
                        .AsQueryable();
            /*foreach (var a in query)
            {
                if (a.CreatorUserId.HasValue)
                {
                    a.CreatorUserName = await WorkScope.GetAll<User>().Where(s => s.Id == a.CreatorUserId).Select(s => s.FullName).FirstOrDefaultAsync();
                }
                switch (a.EntityType)
                {
                    case EntityDefault.Invoice:
                        a.EntityName = (await WorkScope.GetAsync<Invoice>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Client:
                        a.EntityName = (await WorkScope.GetAsync<Client>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Contract:
                        a.EntityName = (await WorkScope.GetAsync<Contract>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Deal:
                        a.EntityName = (await WorkScope.GetAsync<Deal>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Project:
                        a.EntityName = (await WorkScope.GetAsync<Project>(a.EntityId)).Name;
                        break;
                }
                var user = await WorkScope.GetAll<User>().Where(s => s.Id == a.UserId).FirstOrDefaultAsync();
                a.FullName = user?.FullName;
            }*/
            // var query2 = query.AsQueryable().GetGridResultSync(query.AsQueryable(), input.gridParam);
            /*var result = query.WhereIf(!input.SearchUser.IsNullOrWhiteSpace(), s => s.FullName.Contains(input.SearchUser, StringComparison.OrdinalIgnoreCase))
                        .WhereIf(!input.SearchEntity.IsNullOrWhiteSpace(), s => s.EntityName.Contains(input.SearchEntity, StringComparison.OrdinalIgnoreCase));*/
            return query.GetGridResultSync(query, input.Param);
        }

        [HttpGet]
        public async Task<List<GetEntityDto>> GetAllEntity()
        {
            var result = new List<GetEntityDto>();
            var searchDeal = await WorkScope.GetAll<Deal>().Select(s => new GetEntityDto { Id = s.Id, Name = s.Name, EntityType = EntityDefault.Deal }).ToListAsync();
            var searchContract = await WorkScope.GetAll<Contract>().Select(s => new GetEntityDto { Id = s.Id, Name = s.Name, EntityType = EntityDefault.Contract }).ToListAsync();
            var searchProject = await WorkScope.GetAll<Project>().Select(s => new GetEntityDto { Id = s.Id, Name = s.Name, EntityType = EntityDefault.Project }).ToListAsync();
            var searchInvoice = await WorkScope.GetAll<Invoice>().Select(s => new GetEntityDto { Id = s.Id, Name = s.Name, EntityType = EntityDefault.Invoice }).ToListAsync();
            var searchClient = await WorkScope.GetAll<Client>().Select(s => new GetEntityDto { Id = s.Id, Name = s.Name, EntityType = EntityDefault.Client }).ToListAsync();
            var searchContact = await WorkScope.GetAll<Contact>().Select(s => new GetEntityDto { Id = s.Id, Name = s.Name, EntityType = EntityDefault.Contact }).ToListAsync();

            result.AddRange(searchDeal);
            result.AddRange(searchContract);
            result.AddRange(searchProject);
            result.AddRange(searchInvoice);
            result.AddRange(searchClient);
            result.AddRange(searchContact);
            return result;
        }
        [HttpGet]
        public async Task<GetAssignmentDetailDto>GetAssignmentById(long id)
        {
            var item = WorkScope.Get<Assignment>(id);
            var itemEntityInfor = WorkScope.GetAll<EntityAssignment>()
                .Where(x => x.AssignmentId == item.Id)
                .Select(x => new { x.EntityType, x.EntityName, x.EntityId ,x.NameOfEntity})
                .First();
            var userAssignmentId = WorkScope.GetAll<UserAssignment>().Where(x=>x.AssignmentId ==item.Id).FirstOrDefault();
            var itemUserInfor = WorkScope.Get<User>(userAssignmentId.UserId);

            return new GetAssignmentDetailDto
            {
                Id = item.Id,
                IsActive = item.IsActive,
                Name = item.Name,
                Status = item.Status,
                Priority = item.Priority,
                Deadline = item.Deadline,
                Description = item.Description,
                CreatorUserId = item.CreatorUserId,
                EntityId = itemEntityInfor.EntityId,
                EntityName = itemEntityInfor.NameOfEntity,
                EntityType = itemEntityInfor.EntityType,
                EntityTypeName = itemEntityInfor.EntityName,
                User = new UserDetailDto
                {
                    FullName = itemUserInfor.UserName,
                    UserId = itemUserInfor.Id
                }
            };
        }
        [HttpGet]
        public async Task<List<GetListAssignmentDetailDto>> GetAssignmentByEntityId(EntityDefault EntityType, long EntityId)
        {
            var query = from a in WorkScope.GetAll<Assignment>()
                        join ua in WorkScope.GetAll<UserAssignment>() on a.Id equals ua.AssignmentId into tt
                        join e in WorkScope.GetAll<EntityAssignment>() on a.Id equals e.AssignmentId
                        where e.EntityType == EntityType && e.EntityId == EntityId
                        select new GetListAssignmentDetailDto
                        {
                            Id = a.Id,
                            IsActive = a.IsActive,
                            Name = a.Name,
                            Status = a.Status,
                            Priority = a.Priority,
                            Deadline = a.Deadline,
                            Description = a.Description,
                            CreatorUserId = a.CreatorUserId,
                            EntityId = e.EntityId,
                            EntityTypeName = e.EntityName,
                            EntityType = e.EntityType,
                            Users = tt.Select(s => new UserDetailDto
                            {
                                UserId = s.UserId
                            }).ToList()
                        };
            var result = await query.ToListAsync();
            foreach (var a in result)
            {
                if (a.CreatorUserId.HasValue)
                {
                    a.CreatorUserName = await WorkScope.GetAll<User>().Where(s => s.Id == a.CreatorUserId).Select(s => s.FullName).FirstOrDefaultAsync();
                }
                switch (a.EntityType)
                {
                    case EntityDefault.Invoice:
                        a.EntityName = (await WorkScope.GetAsync<Invoice>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Client:
                        a.EntityName = (await WorkScope.GetAsync<Client>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Contract:
                        a.EntityName = (await WorkScope.GetAsync<Contract>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Deal:
                        a.EntityName = (await WorkScope.GetAsync<Deal>(a.EntityId)).Name;
                        break;
                    case EntityDefault.Project:
                        a.EntityName = (await WorkScope.GetAsync<Project>(a.EntityId)).Name;
                        break;
                }
                var listUser = a.Users.ToList();
                foreach (var u in listUser)
                {
                    var user = await WorkScope.GetAll<User>().Where(s => s.Id == u.UserId).FirstOrDefaultAsync();
                    u.FullName = user?.FullName;
                }
                a.Users = listUser;

            }
            return result;
        }

        [HttpPost]
        public async Task<AssignmentDto> Active(EntityDto<long> input)
        {
            var team = await WorkScope.GetAsync<Assignment>(input.Id);
            if (team == null)
                throw new UserFriendlyException(string.Format("Assignment {0} isn't exist", team.Name));
            team.IsActive = true;
            await WorkScope.UpdateAsync(team);
            return ObjectMapper.Map<AssignmentDto>(team);
        }
        [HttpPost]
        public async Task<AssignmentDto> Deactive(EntityDto<long> input)
        {
            var team = await WorkScope.GetAsync<Assignment>(input.Id);
            if (team == null)
                throw new UserFriendlyException(string.Format("Assignment {0} isn't exist", team.Name));
            team.IsActive = false;
            await WorkScope.UpdateAsync(team);
            return ObjectMapper.Map<AssignmentDto>(team);
        }

        [HttpDelete]
        public async System.Threading.Tasks.Task Delete(EntityDto<long> input)
        {
            var hasAssignment = await WorkScope.GetAll<Assignment>().AnyAsync(s => s.Id == input.Id);
            var hasUserAssignment =  WorkScope.GetAll<UserAssignment>().Where(s => s.AssignmentId == input.Id);
            var hasEntityAssignment =  WorkScope.GetAll<EntityAssignment>().Where(s => s.AssignmentId == input.Id);

            if (!hasAssignment)
            {
                throw new UserFriendlyException(string.Format("Assignment Id {0} isn't exist", input.Id));
            }
            /* if (hasUserAssignment)
             {
                 throw new UserFriendlyException(string.Format("This assignment in user assignment, you can't delete it"));
             }
             if (hasEntityAssignment)
             {
                 throw new UserFriendlyException(string.Format("This assignment in entity assignment, you can't delete it"));
             }*/
            foreach(var i in hasUserAssignment)
            {
                await WorkScope.DeleteAsync(i);
            }
            foreach (var i in hasEntityAssignment)
            {
                await WorkScope.DeleteAsync(i);
            }
            await WorkScope.DeleteAsync<Assignment>(input.Id);

        }
    }
}
