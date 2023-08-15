using Abp.Application.Services.Dto;
using Abp.UI;
using CRM.APITeamMembers.TeamRoles.Dto;
using CRM.Entities;
using CRM.Extension;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CRM.APITeamMembers.TeamRoles
{
    [AbpAuthorize]
    public class TeamPermissionAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<TeamPermissionDto> Save(TeamPermissionDto input)
        {
            var isExist = await WorkScope.GetAll<TeamPermission>().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            if (isExist)
            {
                throw new UserFriendlyException(string.Format("Role {0} is already exist", input.Name));
            }
            if (input.Id <= 0)
            {
                var item = ObjectMapper.Map<TeamPermission>(input);
                input.Id = await WorkScope.InsertAndGetIdAsync(item);
            }
            else // update
            {
                var item = await WorkScope.GetAsync<TeamPermission>(input.Id);
                ObjectMapper.Map<TeamPermissionDto, TeamPermission>(input, item);
                await WorkScope.UpdateAsync(item);
            }
            return input;
        }
        [HttpDelete]
        public async System.Threading.Tasks.Task Delete(EntityDto<long> input)
        {
            var hasRole = await WorkScope.GetAll<TeamPermission>().AnyAsync(s => s.Id == input.Id);
            var hasUserTeamRole = await WorkScope.GetAll<UserTeamPermission>().AnyAsync(s => s.TeamPermissionId == input.Id);
            if (!hasRole)
            {
                throw new UserFriendlyException(string.Format("Role Id {0} isn't exist", input.Id));
            }
            if (hasUserTeamRole)
            {
                throw new UserFriendlyException(string.Format("This role in team, you can't delete it", input.Id));
            }
            await WorkScope.DeleteAsync<TeamPermission>(input.Id);
        }
        [HttpGet]
        public async Task<List<TeamPermissionDto>> GetAll()
        {
            return await WorkScope.GetAll<TeamPermission>().Select(s => new TeamPermissionDto
            {
                Id = s.Id,
                Name = s.Name
            }).ToListAsync();
        }

    }

}
