using Abp.Application.Services.Dto;
using Abp.UI;
using CRM.APITeamMembers.Teams.Dto;
using CRM.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CRM.APITeamMembers.Teams
{
    [AbpAuthorize]
    public class TeamAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<TeamDto> Save(TeamDto input)
        {
            var isExist = await WorkScope.GetAll<Team>().AnyAsync(s => s.Name == input.Name && s.Id != input.Id);
            if (isExist)
            {
                throw new UserFriendlyException(string.Format("Team {0} is already exist", input.Name));
            }
            if (input.Id <= 0)
            {
                var item = ObjectMapper.Map<Team>(input);
                input.Id = await WorkScope.InsertAndGetIdAsync(item);
            }
            else // update
            {
                var item = await WorkScope.GetAsync<Team>(input.Id);
                ObjectMapper.Map<TeamDto, Team>(input, item);
                await WorkScope.UpdateAsync(item);
            }
            return input;
        }
        [HttpDelete]
        public async System.Threading.Tasks.Task Delete(EntityDto<long> input)
        {
            var hasTeam = await WorkScope.GetAll<Team>().AnyAsync(s => s.Id == input.Id);
            var hasUserTeam = await WorkScope.GetAll<UserTeam>().AnyAsync(s => s.TeamId == input.Id);
            if (!hasTeam)
            {
                throw new UserFriendlyException(string.Format("Team Id {0} isn't exist", input.Id));
            }
            if (hasUserTeam)
            {
                throw new UserFriendlyException(string.Format("This team in user team, you can't delete it"));
            }
            await WorkScope.DeleteAsync<Team>(input.Id);
        }
        [HttpGet]
        public async Task<List<TeamDto>> GetAll()
        {
            return await WorkScope.GetAll<Team>().Select(s => new TeamDto
            {
                Id = s.Id,
                Name = s.Name,
                IsActive = s.IsActive
            }).ToListAsync();
        }
        [HttpPost]
        public async Task<TeamDto> Active(EntityDto<long> input)
        {
            var team = await WorkScope.GetAsync<Team>(input.Id);
            if (team == null)
                throw new UserFriendlyException(string.Format("Team {0} isn't exist", team.Name));
            team.IsActive = true;
            await WorkScope.UpdateAsync(team);
            return ObjectMapper.Map<TeamDto>(team);
        }
        [HttpPost]
        public async Task<TeamDto> DeActive(EntityDto<long> input)
        {
            var team = await WorkScope.GetAsync<Team>(input.Id);
            if (team == null)
                throw new UserFriendlyException(string.Format("Team {0} isn't exist", team.Name));
            team.IsActive = false;
            await WorkScope.UpdateAsync(team);
            return ObjectMapper.Map<TeamDto>(team);
        }
    }
}
