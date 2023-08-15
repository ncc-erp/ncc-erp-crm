using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Projects.Dto
{
    [AutoMapTo(typeof(Project))]
    public class SaveProjectDto : EntityDto<long>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public ProjectType Type { get; set; }
        public long ClientId { get; set; }
        public ProjectStatus Status { get; set; }
        public string Description { get; set; }
        public List<long> Users { get; set; }
        public long? DealId { get; set; }
    }
}
