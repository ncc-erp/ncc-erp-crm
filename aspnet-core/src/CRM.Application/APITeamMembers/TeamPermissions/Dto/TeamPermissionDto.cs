using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;

namespace CRM.APITeamMembers.TeamRoles.Dto
{
    [AutoMapTo(typeof(TeamPermission))]
    public class TeamPermissionDto: EntityDto<long>
    {
        public string Name { get; set; }
    }
}
