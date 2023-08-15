using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.APITeamMembers.Teams.Dto
{
    [AutoMap(typeof(Team))]
    public class TeamDto : EntityDto<long>
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }

}
