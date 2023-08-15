using Abp.Application.Services.Dto;
using CRM.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APIAssignments.Assignments.Dto
{
    public class CreateAssignmentDto : EntityDto<long>
    {
        public AssignmentStatus Status { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public List<EntityAssignmentDto> EntityAssignmentDtos { get; set; }
        public List<long> UserIds { get; set; }
    }
    public class EntityAssignmentDto
    {
        public long EntityId { get; set; }
        public EntityDefault EntityType { get; set; }
    }
    
}
