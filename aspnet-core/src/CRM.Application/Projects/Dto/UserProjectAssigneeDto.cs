using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Projects.Dto
{
    public class UserProjectAssigneeDto : EntityDto<long>
    {
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
