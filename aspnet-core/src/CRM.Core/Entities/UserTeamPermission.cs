using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class UserTeamPermission : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(UserTeamId))]
        public UserTeam UserTeam { get; set; }
        public long UserTeamId { get; set; }
        [ForeignKey(nameof(TeamPermissionId))]
        public TeamPermission TeamPermission { get; set; }
        public long TeamPermissionId { get; set; }
        public int? TenantId { get; set; }
    }
}
