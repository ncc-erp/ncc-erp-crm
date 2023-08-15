using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static CRM.Enums.StatusEnum;

namespace CRM.Entities
{
    public class EntityAssignment : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(AssignmentId))]
        public Assignment Assignment { get; set; }
        public long AssignmentId { get; set; }
        public long EntityId { get; set; }
        public EntityDefault EntityType { get; set; }
        public string EntityName { get; set; }
        public string NameOfEntity { get; set; }
        public int? TenantId { get; set; }
    }
}
