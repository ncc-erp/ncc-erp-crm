using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class DealDetail : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public long DealId { get; set; }
        [ForeignKey(nameof(DealId))]
        public Deal Deal { get; set; }
        public long SkillId { get; set; }
        [ForeignKey(nameof(SkillId))]
        public Skill Skill { get; set;}
        public long LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public Level Level { get; set; }
        public int Quantity { get; set; }
    }
}
