using Abp.Authorization;
using CRM.APITeamMembers.Teams.Dto;
using CRM.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CRM.Authorization.Users;
using Microsoft.EntityFrameworkCore;
using CRM.IoC;
using Abp.Zero;
using Abp.Modules;
using CRM.EntityFrameworkCore;
using CRM.APITeamMembers.UserTeams.Dto;
using Abp.Application.Services.Dto;
using Abp.UI;
//using DevExpress.Xpo;

namespace CRM.APITeamMembers.UserTeams
{
    [AbpAuthorize]
    public class UserTeamAppService : CRMAppServiceBase
    {
        [HttpPost]
        public async Task<CreateUserTeamDto> Create(CreateUserTeamDto input)
        {
            foreach(var ut in input.UserTeamPermissions)
            {
                var userTeam = new UserTeam
                {
                    TeamId = input.TeamId,
                    UserId = ut.UserId,
                    TeamMember = ut.TeamMember
                };
              
                var userTeamId =  WorkScope.InsertAndGetId<UserTeam>(userTeam);
              
                foreach (var t in ut.TeamPermissionIds)
                {
                    var teamPermission = new UserTeamPermission
                    {
                        UserTeamId = userTeamId,
                        TeamPermissionId = t
                    };
                    await WorkScope.InsertAsync<UserTeamPermission>(teamPermission);
                }
            }
            return input;
        }

        [HttpPost]
        public async Task<CreateUserTeamDto> Update(CreateUserTeamDto input)
        {
            var currentUserTeams = WorkScope.GetAll<UserTeam>().Where(s => s.TeamId == input.TeamId).ToList();
            var newUserTeams = input.UserTeamPermissions.Select(s=>s.UserId).ToList();
            var deleteUserTeams = currentUserTeams.Where(s => !newUserTeams.Contains(s.UserId)).ToList();
           // var deleteUserTeams = currentUserTeams.Select(s=>s.UserId).Except(newUserTeams);
            var newUsers = newUserTeams.Except(currentUserTeams.Select(s => s.UserId));
            var oldUsers = newUserTeams.Intersect(currentUserTeams.Select(s => s.UserId)); 
            //delete 
            foreach (var u in deleteUserTeams)
            {
                await WorkScope.DeleteAsync<UserTeam>(u.Id);
                var teamPermissionIds =  WorkScope.GetAll<UserTeamPermission>().Where(s => s.UserTeam.UserId == u.UserId);
                foreach (var t in teamPermissionIds)
                {
                    await WorkScope.DeleteAsync<UserTeamPermission>(t.Id);
                }
            }

            //insert
            var insertUserTeams = input.UserTeamPermissions.Where(s => newUsers.Contains(s.UserId));
            foreach (var ut in insertUserTeams)
            {
                var userTeam = new UserTeam
                {
                    UserId = ut.UserId,
                    TeamId = input.TeamId,
                    TeamMember = ut.TeamMember
                };
                var userTeamId = await WorkScope.InsertAndGetIdAsync<UserTeam>(userTeam);
                foreach (var t in ut.TeamPermissionIds)
                {
                    var userteamPermission = new UserTeamPermission
                    {
                        UserTeamId = userTeamId,
                        TeamPermissionId = t
                    };
                    await WorkScope.InsertAsync<UserTeamPermission>(userteamPermission);
                }
            }

            //update 
            var updateUserTeams = input.UserTeamPermissions.Where(s => oldUsers.Contains(s.UserId));

            foreach (var ut in updateUserTeams)
            {
                var userTeam = await WorkScope.GetAll<UserTeam>().Where(s => s.UserId == ut.UserId).FirstOrDefaultAsync();
                userTeam.TeamMember = ut.TeamMember;
                await WorkScope.UpdateAsync<UserTeam>(userTeam);

                var listOldPermission = WorkScope.GetAll<UserTeamPermission>().Where(s=>s.UserTeam.UserId == ut.UserId && s.UserTeam.TeamId == input.TeamId).ToList();
                var listNewPermission = updateUserTeams.Where(s => s.UserId == ut.UserId).Select(s => s.TeamPermissionIds).FirstOrDefault();
                var insertPermission = listNewPermission.Except(listOldPermission.Select(s => s.TeamPermissionId));
                var deletePermission = listOldPermission.Select(s=>s.TeamPermissionId).Except(listNewPermission);

                var userTeamId = listOldPermission.Where(s => s.UserTeam.UserId == ut.UserId).Select(s => s.UserTeamId).FirstOrDefault();
                foreach (var t in insertPermission)
                {
                    var userteamPermission = new UserTeamPermission
                    {
                        UserTeamId = userTeamId,
                        TeamPermissionId = t
                    };
                    await WorkScope.InsertAsync<UserTeamPermission>(userteamPermission);
                }
                var deleteUserTeamPermission = listOldPermission.Where(s => deletePermission.Contains(s.TeamPermissionId));
                foreach(var d in deleteUserTeamPermission)
                {
                    await WorkScope.DeleteAsync<UserTeamPermission>(d.Id);
                }
            }

            return input;
        }
        [HttpGet]
        public async Task<List<GetUserTeamDto>> GetAll()
        {
            var query = WorkScope.GetAll<UserTeamPermission>().Where(s => s.UserTeam.Team.IsActive == true)
                .Select(s => new
                {
                    PermissionName = s.TeamPermission.Name,
                    PermissionId = s.TeamPermission.Id,
                    s.UserTeam.TeamId,
                    s.UserTeam.UserId,
                    s.UserTeam.TeamMember,
                    TeamName = s.UserTeam.Team.Name,
                    s.UserTeam.User.UserName
                }).GroupBy(s => new { s.TeamId, s.TeamName })
                .Select(s => new GetUserTeamDto
                {
                    TeamName = s.Key.TeamName,
                    TeamId = s.Key.TeamId,
                    GetUserTeamPermissionDtos = s.Select(t => new
                    {
                        t.UserName,
                        t.UserId,
                        t.TeamMember,
                        t.PermissionName,
                        t.PermissionId
                    }).GroupBy(t => new { t.UserId, t.UserName, t.TeamMember })
                    .Select(k => new GetUserTeamPermissionDto
                    {
                        TeamMember = k.Key.TeamMember,
                        UserName = k.Key.UserName,
                        UserId = k.Key.UserId,
                        GetTeamPermissionDtos = k.Select(u => new GetTeamPermissionDto
                        {
                            PermissionName = u.PermissionName,
                            TeamPermissionId = u.PermissionId
                        })
                    })
                });
            return await query.ToListAsync();

        }

        [HttpGet]
        public async Task<List<GetUserTeamDto>> Get(long Id)
        {
            var query = WorkScope.GetAll<UserTeamPermission>().Where(s => s.UserTeam.TeamId == Id)
                .Select(s => new
                {
                    PermissionName = s.TeamPermission.Name,
                    PermissionId = s.TeamPermission.Id,
                    s.UserTeam.TeamId,
                    s.UserTeam.UserId,
                    s.UserTeam.TeamMember,
                    TeamName = s.UserTeam.Team.Name,
                    s.UserTeam.User.UserName
                }).GroupBy(s => new { s.TeamId, s.TeamName })
                .Select(s => new GetUserTeamDto
                {
                    TeamName = s.Key.TeamName,
                    TeamId = s.Key.TeamId,
                    GetUserTeamPermissionDtos = s.Select(t => new
                    {
                        t.UserName,
                        t.UserId,
                        t.TeamMember,
                        t.PermissionName,
                        t.PermissionId
                    }).GroupBy(t => new { t.UserId, t.UserName, t.TeamMember })
                    .Select(k => new GetUserTeamPermissionDto
                    {
                        TeamMember = k.Key.TeamMember,
                        UserName = k.Key.UserName,
                        UserId = k.Key.UserId,
                        GetTeamPermissionDtos = k.Select(u => new GetTeamPermissionDto
                        {
                            PermissionName = u.PermissionName,
                            TeamPermissionId = u.PermissionId
                        })
                    })
                });
            return await query.ToListAsync();

        }
    }
}
