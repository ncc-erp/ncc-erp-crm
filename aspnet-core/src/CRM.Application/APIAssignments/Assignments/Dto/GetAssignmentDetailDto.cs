using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.APIAssignments.Assignments.Dto
{
    public class GetAssignmentDetailDto : EntityDto<long>
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
        public UserDetailDto User { get; set; }
    }

    public class UserDetailDto
    {
        public string FullName { get; set; }
        public long UserId { get; set; }
    }
    public class GetListAssignmentDetailDto : EntityDto<long>
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
        public List<UserDetailDto> Users { get; set; }
    }
}
