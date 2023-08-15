using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace CRM.Entities
{
    public class DealComment : FullAuditedEntity<long>, IMayHaveTenant
    {
        [ForeignKey(nameof(DealId))]
        public Deal Deal { get; set; }
        [Required]
        public long DealId { get; set; }
        public string Content { get; set; }
        public int? TenantId { get; set; }
        public long? ParentId { get; set; }
    }
}
