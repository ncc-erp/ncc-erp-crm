using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APIAssignments.Assignments.Dto
{
    [AutoMap(typeof(Assignment))]
    public class AssignmentDto : EntityDto<long>
    {
        public AssignmentStatus Status { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
    }
}
