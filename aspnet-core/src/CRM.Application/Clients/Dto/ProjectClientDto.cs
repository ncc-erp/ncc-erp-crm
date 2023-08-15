using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Clients.Dto
{
    public class ProjectClientDto : EntityDto<long>
    {
        public string Name { get; set; }
        public ProjectType Type { get; set; }
        public string Code { get; set; }
        public ProjectStatus Status { get; set; }
        public string Description { get; set; }
    }
}
