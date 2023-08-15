using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.APITeamMembers.UserTeams.Dto
{
    [AutoMapTo(typeof(UserTeam))]
    public class UserTeamDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public long TeamId { get; set; }
    }
   
}
