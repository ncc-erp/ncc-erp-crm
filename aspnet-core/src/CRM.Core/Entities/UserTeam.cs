using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using CRM.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Entities
{
    public class UserTeam : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(TeamId))]
        public Team Team { get; set; }
        public long TeamId { get; set; }
        public TeamMember TeamMember { get; set; }
        public int? TenantId { get; set; }
    }
}
