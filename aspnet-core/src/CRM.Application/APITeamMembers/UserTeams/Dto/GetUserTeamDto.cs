using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APITeamMembers.UserTeams.Dto
{
    public class GetUserTeamDto
    {
        public long TeamId { get; set; }
        public string TeamName { get; set; }
        public IEnumerable<GetUserTeamPermissionDto> GetUserTeamPermissionDtos { get; set; }
    }
    public class GetUserTeamPermissionDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public TeamMember TeamMember { get; set; }
        public IEnumerable<GetTeamPermissionDto> GetTeamPermissionDtos { get; set; }
    }
    public class GetTeamPermissionDto
    {
        public long TeamPermissionId { get; set; }
        public string PermissionName { get; set; }
    }
}
