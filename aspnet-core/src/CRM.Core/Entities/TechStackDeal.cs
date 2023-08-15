using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class TechStackDeal : FullAuditedEntity<long>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public int Quantity { get; set; }
        public long TechStackId { get; set; }
        [ForeignKey(nameof(TechStackId))]
        public TechStack TechStack { get; set; }
        public long DealId { get; set; }
        [ForeignKey(nameof(DealId))]
        public Deal Deal { get; set; }
    }
}
