using Abp.Application.Services.Dto;
using CRM.Anotations;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APIAssignments.Assignments.Dto
{
    public class PagingAssignmentDto
    {
        public int TotalCount { get; set; }
        public IEnumerable<GetAssignmentDto> Items { get; set; }
    }
    public class GetAssignmentDto : EntityDto<long>
    {
        public AssignmentStatus Status { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public DateTime Deadline { get; set; }
        public string Description { get; set; }
        public Priority Priority { get; set; }
        public string CreatorUserName { get; set; }
        public long? CreatorUserId { get; set; }
        public long EntityId { get; set; }
        public EntityDefault EntityType { get; set; }
        public string EntityTypeName { get; set; }
        public string EntityName { get; set; }
        public string FullName { get; set; }
        public long UserId { get; set; }
    }
}
