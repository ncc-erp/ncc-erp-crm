using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CRM.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class UserAssignment : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(AssignmentId))]
        public Assignment Assignment { get; set; }
        public long AssignmentId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(CreatorUserId))]
        public User User2 { set; get; }
        public int? TenantId { get; set; }
    }
}

