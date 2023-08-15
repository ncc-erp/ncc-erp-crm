using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CRM.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class ProjectAssignee : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        [Required]
        public long UserId { get; set; }
        [ForeignKey(nameof(ProjectId))]
        public Project Project { get; set; }
        [Required]
        public long ProjectId { get; set; }
        public int? TenantId { get; set; }
    }
}
