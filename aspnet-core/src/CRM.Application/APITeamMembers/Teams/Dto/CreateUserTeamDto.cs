using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APITeamMembers.Teams.Dto
{
    public class CreateUserTeamDto
    {
        public long TeamId { get; set; }
        public List<UserTeamPermissionDto> UserTeamPermissions { get; set; }
    }

    public class UserTeamPermissionDto
    {
        public long UserId { get; set; }
        public TeamMember TeamMember { get; set; }
        public List<long> TeamPermissionIds { get; set; }
    }

}
